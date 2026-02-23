# Bingo Configuration Endpoints

This document defines the API endpoints available for managing Bingo configurations, including team settings, items, and webhooks.

## 1. Update Bingo Team Configuration

Updates or creates the team configuration for a specific character. There is only one team configuration per character name.

- **Method**: `PUT`
- **URL**: `/api/BingoConfig?character={CHARACTER_NAME}`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "teamName": "The Champions",
  "teamNameColor": "#FFD700",
  "dateTimeColor": "#FFFFFF"
}
```
- **Description**: Performs an "upsert". If a configuration already exists for the given character (case-insensitive), it will be updated. Otherwise, a new one will be created.

---

## 2. Bulk Update Bingo Team Configurations

Updates or creates team configurations for multiple characters at once.

- **Method**: `POST`
- **URL**: `/api/BingoConfig/teams`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
[
  {
    "characterName": "PlayerOne",
    "teamName": "The Champions",
    "teamNameColor": "#FFD700",
    "dateTimeColor": "#FFFFFF"
  },
  {
    "characterName": "PlayerTwo",
    "teamName": "The Challengers",
    "teamNameColor": "#C0C0C0",
    "dateTimeColor": "#000000"
  }
]
```
- **Description**: Performs a bulk "upsert". For each character in the list, if a configuration already exists, it will be updated. Otherwise, a new one will be created.

---

## 3. Add/Update Bingo Items

Adds a list of items to the bingo pool or updates their sources if they already exist.

- **Method**: `POST`
- **URL**: `/api/BingoConfig/items`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
[
  {
    "name": "Twisted bow",
    "source": "Chambers of Xeric"
  },
  {
    "name": "Scythe of vitur",
    "source": "Theatre of Blood"
  },
  {
    "name": "Tbow",
    "source": ""
  }
]
```
- **Description**: Merges on `name` (case-insensitive). If an item with the same name exists, its `source` will be updated. If it doesn't exist, it will be added to the database.

---

## 4. Add Bingo Webhook

Adds a new Discord webhook for a specific character.

- **Method**: `POST`
- **URL**: `/api/BingoConfig/webhooks`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "characterName": "PlayerOne",
  "webhookUrl": "https://discord.com/api/webhooks/..."
}
```
- **Description**: Adds a new record to the webhooks table for the specified character. Multiple webhooks can be added for the same character.

---

## 5. Get Bingo Configuration

Fetches all configuration (webhooks, items, and team settings) for a specific character.

- **Method**: `POST`
- **URL**: `/api/BingoConfig?character={CHARACTER_NAME}`
- **Body**: (Optional empty object `{}`)
- **Description**: Returns the current configuration including all registered webhooks for the character, all items in the bingo pool, and the team configuration if it exists.
