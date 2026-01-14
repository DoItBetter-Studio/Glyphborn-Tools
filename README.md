# Glyphborn Tools

Glyphborn Tools is a focused suite of editors used to build the world of **Glyphborn**.  
Each tool has a single responsibility, a clean boundary, and a deterministic workflow.  
Nothing is hidden, nothing is ambiguous — every subsystem is intentional.

---

## 🧱 Overview

This repository contains the core tools used to author and assemble the Glyphborn world:

- **Mapper** — The map editor.  
  Used to paint tiles, define collision, manage tilesets, and export per‑map runtime data.

- **Matrix Editor** (planned) — The world orchestrator.  
  Used to place maps on the world grid, define adjacency, attach scripts, assign entities, and export final world data.

These tools are **not** the game engine. They are the authoring environment that produces the data the engine consumes.

---

## 🎨 Mapper

The **Mapper** is a deterministic, editor‑only tool for creating maps.  
Its responsibilities include:

- Multi‑layer tile painting  
- Collision authoring  
- Tileset loading (regional, local, interior)  
- Editing areas composed of one or more maps  
- Saving/loading authoring documents (`.gbm`)  
- Exporting per‑map binary data (`geometry.bin`, `collision.bin`)  
- Using internal dialogs instead of OS file pickers  
- Operating entirely within its own asset roots

The Mapper **does not**:

- Place maps in the world  
- Handle entities  
- Attach scripts  
- Manage world metadata  
- Perform global exports  

It is intentionally narrow.  
It creates **maps**, nothing more.

---

## 🌐 Matrix Editor (World Orchestrator)

The **Matrix Editor** is the next major tool in the pipeline.  
Its role is to assemble the world from finalized map exports.

Planned responsibilities:

- World grid layout  
- Map placement and adjacency  
- Region and world metadata  
- Entity placement  
- Script connections  
- Transition logic  
- Streaming boundaries  
- Final world export for the engine

The Matrix Editor works with **finalized map data**, not raw tiles or authoring formats.  
It handles **world data**, not assets.

---

## 🧠 Philosophy

Glyphborn Tools are built on a few core principles:

### **Determinism**
Outputs are stable and reproducible.  
No timestamps, no hidden metadata, no surprises.

### **Separation of Concerns**
- Mapper creates maps  
- Matrix Editor assembles the world  
- Engine consumes final data  

No tool crosses its boundary.

### **Purity**
Editor formats are explicit.  
Export formats are tight and final.  
Nothing is stored “just in case.”

### **Self‑Contained Tools**
No OS dialogs.  
No external dependencies.  
Each tool knows its own roots and libraries.

---

## 🚧 Project Status

- **Mapper:**  
  Structurally complete and usable.  
  A final “tightening pass” on save formats will occur once enough real content exists.

- **Matrix Editor:**  
  Planned.  
  Will become the authoritative world‑building tool once map content is mature.

- **Tool Assets:**  
  Used only for editor UI (icons, etc.).  
  Not part of the game’s runtime asset pipeline.

---

## 📜 License

To be determined.  
This project is part of the Glyphborn ecosystem.

---

