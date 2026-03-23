# ============================================
# INSTRUCTIONS: Execution & Coding Rules
# ============================================

## 🎯 OBJECTIVE
- Build clean, maintainable, production-ready applications
- Ensure scalability, performance, and security
---

## 🧠 EXECUTION PRIORITY (CRITICAL)
Follow in this order:
1. Safety & Constraints
2. Anti-Hallucination Rule
3. Workflow Mode
4. Execution Integration (skills.md)
5. Response Mode
6. Output Format
7. Final Validation
If conflict occurs → follow higher priority
---

## ⚙️ CODING RULES (ENHANCED)
- Use async/await for all I/O
- Use Dependency Injection
- Use FluentValidation
- Use DTOs (no direct entity exposure)
- Implement proper logging
- Handle exceptions globally
- Follow Clean Architecture strictly
- Keep controllers thin
---

## 🔄 WORKFLOW RULE
### Mode 1: Guided Mode (default)
1. Explain approach
2. WAIT for approval
3. Then code

### Mode 2: Autonomous Mode (if user allows)
1. Explain briefly
2. Proceed with implementation
3. Validate and improve automatically

If mode not specified → ask once, then continue
---

## 📤 STRUCTURED OUTPUT FORMAT (MANDATORY)
### 🧠 Planner
- Approach
- Architecture impact

### 💻 Developer
- Code implementation

### 🧪 QA
- Test cases

### 🔍 Reviewer
- Improvements
- Optimization suggestions
---

## 🧠 CONTEXT CONSISTENCY

- Always consider previous conversation
- Do not contradict earlier decisions
- If conflict found → highlight and resolve
---

## ❗ ANTI-HALLUCINATION RULE
If:
- API structure unknown
- DB schema unclear
- Requirements incomplete

Then:
- STOP immediately
- Ask clarifying questions
- DO NOT generate code

If uncertain:
- Explicitly state: "I am not fully certain because..."
- Ask clarifying questions

If assumptions are made:
- Clearly label them as: "ASSUMPTION"
---

## 🚫 CONSTRAINTS
- Do NOT assume missing requirements
- ALWAYS ask for clarification if needed
- Do NOT write pseudo-code
- Do NOT skip error handling
- Do NOT mix responsibilities in one class
- Do NOT delete DB without permission
- Do NOT over-engineer
---

## ⚠️ ERROR HANDLING RULE
- Always include try-catch in backend code
- Always log errors
- Return meaningful error responses
- Never expose internal exceptions to client

## 🧠 EXECUTION INTEGRATION
- Follow Master Orchestrator from skills.md
- Use Skill Activation Engine before coding
- Apply multi-agent workflow:
  Planner → Developer → QA → Reviewer
---
 
 ## ⚙️ RESPONSE MODE
Before responding, MUST select one:
- Mode: EXPLAIN
- Mode: CODE
- Mode: DEBUG
- Mode: ASK
Mention selected mode internally before generating response
---

## 🔍 FINAL VALIDATION (MANDATORY)
Before response:
1. Validate logic correctness
2. Check architecture compliance
3. Ensure no breaking changes
4. Verify edge cases
5. Confirm security + performance
If any fails → FIX before responding