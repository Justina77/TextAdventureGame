import express from "express";
import cors from "cors";
import dotenv from "dotenv";

dotenv.config();

const app = express();

app.use(cors());
app.use(express.json());

const PORT = process.env.PORT || 3000;
const INWORLD_API_KEY = process.env.INWORLD_API_KEY;
const INWORLD_AUTH_TYPE = process.env.INWORLD_AUTH_TYPE || "Bearer";

if (!INWORLD_API_KEY || INWORLD_API_KEY === "PASTE_YOUR_INWORLD_KEY_HERE") {
  console.error("Missing INWORLD_API_KEY in backend/.env");
  process.exit(1);
}

const NPC_1_SYSTEM_PROMPT = `
You are an NPC in a text-adventure game.

You and the traveler are both inside the game world. The traveler's goal is to kill the dragon. Your role is to answer the traveler's questions correctly, using only the game information below.

Do not immediately explain the full solution unless the traveler directly asks for it.
Wait for the traveler to ask questions.
Answer clearly and briefly.

Game layout:
- room2 is east of room1.
- room3 is east of room2.
- The dungeon is in room3.
- The dragon is in the dungeon.

Goal and prerequisite:
- The main goal is to kill the dragon.
- The only way to kill the dragon is to use the sword.
- There is no other way to kill the dragon.

Object information:
- The sword is in room1.
- object2 is in room1.
- object3 is in room2.
- object4 is in room2.
- object5 is in room2.
- room3 has no objects except the dungeon and the dragon.

Rules for answering:
- If the traveler asks where something is, answer based only on the information above.
- If the traveler asks whether an action is possible, answer yes or no and briefly explain why.
- If the traveler asks about something that is not in the given information, say that you do not know.
- Do not invent new rooms, objects, weapons, or ways to kill the dragon.
- Keep answers short, like an NPC in a text adventure game.
`;

const conversations = new Map();

app.get("/", (req, res) => {
  res.send("Text Adventure Game backend is running.");
});

app.post("/api/npc1", async (req, res) => {
  try {
    const { sessionId, message } = req.body;

    if (!sessionId || !message) {
      return res.status(400).json({
        error: "sessionId and message are required"
      });
    }

    let history = conversations.get(sessionId);

    if (!history) {
      history = [
        {
          role: "system",
          content: NPC_1_SYSTEM_PROMPT
        }
      ];
    }

    history.push({
      role: "user",
      content: message
    });

    const response = await fetch("https://api.inworld.ai/v1/chat/completions", {
      method: "POST",
      headers: {
        "Authorization": `${INWORLD_AUTH_TYPE} ${INWORLD_API_KEY}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        model: "auto",
        messages: history,
        temperature: 0.2,
        max_tokens: 150
      })
    });

    const data = await response.json();

    if (!response.ok) {
      console.error("Inworld API error:", data);

      return res.status(500).json({
        error: "Inworld API error",
        details: data
      });
    }

    const reply = data.choices?.[0]?.message?.content || "I do not know.";

    history.push({
      role: "assistant",
      content: reply
    });

    const systemPrompt = history[0];
    const recentMessages = history.slice(1).slice(-20);

    conversations.set(sessionId, [systemPrompt, ...recentMessages]);

    res.json({
      reply: reply
    });
  } catch (error) {
    console.error("Server error:", error);

    res.status(500).json({
      error: "Server error"
    });
  }
});

app.listen(PORT, () => {
  console.log(`Backend running on http://localhost:${PORT}`);
});