// src/hooks/useChatSignalR.js
// ─────────────────────────────────────────────────────────────────────────────
// Custom React hook that encapsulates ALL SignalR logic for one chat panel.
// Each panel (User A / User B) gets its own independent connection instance.
//
// Responsibilities:
//   - Start / stop the SignalR connection
//   - Register server→client event handlers (ReceiveMessage, ChatHistoryLoaded…)
//   - Expose sendMessage() and connectionState to the component
// ─────────────────────────────────────────────────────────────────────────────

import { useState, useEffect, useRef, useCallback } from "react";
import { createHubConnection } from "../services/signalRService";
import * as signalR from "@microsoft/signalr";

/**
 * @param {string} senderId    - Identity of the user who owns this panel
 * @param {string} receiverId  - Identity of the other user
 */
const useChatSignalR = (senderId, receiverId) => {
  const connectionRef = useRef(null);

  const [messages, setMessages]               = useState([]);
  const [connectionState, setConnectionState] = useState("Disconnected");
  const [error, setError]                     = useState(null);

  // ─── Start connection and wire up all server→client events ───────────────
  useEffect(() => {
    if (!senderId || !receiverId) return;

    const connection = createHubConnection();
    connectionRef.current = connection;

    // ── Event: Server broadcasts a new message to the group ──
    connection.on("ReceiveMessage", (messageDto) => {
      setMessages((prev) => {
        // Deduplicate: ignore if MessageId already exists (reconnect edge case)
        if (prev.some((m) => m.messageId === messageDto.messageId)) return prev;
        return [...prev, messageDto];
      });
    });

    // ── Event: Chat history returned for this panel's initial load ──
    connection.on("ChatHistoryLoaded", (history) => {
      setMessages(history);
    });

    // ── Event: Another user connected ──
    connection.on("UserConnected", (userId) => {
      console.info(`[SignalR] User connected to group: ${userId}`);
    });

    // ── Event: Another user disconnected ──
    connection.on("UserDisconnected", (userId) => {
      console.info(`[SignalR] User disconnected: ${userId}`);
    });

    // ── Event: Server-side validation or hub error ──
    connection.on("Error", (message) => {
      setError(message);
      console.error(`[SignalR Hub Error] ${message}`);
    });

    // ── Reconnection state tracking ──
    connection.onreconnecting(() => setConnectionState("Reconnecting"));
    connection.onreconnected(() => {
      setConnectionState("Connected");
      // Re-join the chat group after reconnection
      connection.invoke("JoinChat", senderId, receiverId).catch(console.error);
    });
    connection.onclose(() => setConnectionState("Disconnected"));

    // ── Start the connection ──
    const startConnection = async () => {
      try {
        await connection.start();
        setConnectionState("Connected");
        setError(null);

        // 1. Join the shared chat group
        await connection.invoke("JoinChat", senderId, receiverId);

        // 2. Load conversation history
        await connection.invoke("LoadChatHistory", senderId, receiverId);
      } catch (err) {
        setConnectionState("Disconnected");
        setError("Could not connect to chat server. Retrying…");
        console.error("[SignalR] Start failed:", err);
      }
    };

    startConnection();

    // ── Cleanup: stop connection when component unmounts ──
    return () => {
      connection.stop().catch(console.error);
    };
  }, [senderId, receiverId]); // Re-run only if user identities change

  // ─── Send a message via SignalR ──────────────────────────────────────────
  const sendMessage = useCallback(async (messageText) => {
    const connection = connectionRef.current;

    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
      setError("Not connected. Please wait and try again.");
      return;
    }

    if (!messageText?.trim()) return;

    try {
      await connection.invoke("SendMessage", {
        senderId:    senderId,
        receiverId:  receiverId,
        messageText: messageText.trim(),
      });
    } catch (err) {
      setError("Failed to send message. Please try again.");
      console.error("[SignalR] SendMessage failed:", err);
    }
  }, [senderId, receiverId]);

  return { messages, connectionState, error, sendMessage };
};

export default useChatSignalR;
