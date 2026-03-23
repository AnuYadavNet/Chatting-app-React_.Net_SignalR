// src/services/signalRService.js
// ─────────────────────────────────────────────────────────────────────────────
// Manages the SignalR HubConnection lifecycle.
// This service is intentionally kept framework-agnostic — it returns a plain
// connection object so any React component / hook can subscribe to events.
// ─────────────────────────────────────────────────────────────────────────────

import * as signalR from "@microsoft/signalr";

const HUB_URL = process.env.REACT_APP_HUB_URL || "https://localhost:62401/hubs/chat";

/**
 * Builds and returns a configured (but not yet started) HubConnection.
 *
 * WHY NOT START IT HERE?
 *   Starting is async and needs React lifecycle awareness (useEffect cleanup).
 *   We let the consumer (useChatSignalR hook) call .start() and handle errors.
 */
export const createHubConnection = () => {
  return new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL, {
      accessTokenFactory: () => localStorage.getItem("jwtToken") || "",
      // Allows SignalR to fall back: WebSockets → Server-Sent Events → Long Polling
      transport:
        signalR.HttpTransportType.WebSockets |
        signalR.HttpTransportType.ServerSentEvents |
        signalR.HttpTransportType.LongPolling,
    })
    .withAutomaticReconnect({
      // Custom backoff: 0s, 2s, 5s, 10s, then every 30s
      nextRetryDelayInMilliseconds: (retryContext) => {
        const delays = [0, 2000, 5000, 10000];
        return delays[retryContext.previousRetryCount] ?? 30000;
      },
    })
    .configureLogging(
      process.env.NODE_ENV === "development"
        ? signalR.LogLevel.Information
        : signalR.LogLevel.Warning
    )
    .build();
};
