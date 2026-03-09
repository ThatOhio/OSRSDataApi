# Bingo API Endpoints

This document defines the API endpoints available for managing Bingo configurations, including team settings, items, webhooks, team icons, and activity logs.

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
  "dateTimeColor": "#FFFFFF",
  "teamIcon": "https://example.com/icon.png"
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
    "dateTimeColor": "#FFFFFF",
    "teamIcon": "https://example.com/icon1.png"
  },
  {
    "characterName": "PlayerTwo",
    "teamName": "The Challengers",
    "teamNameColor": "#C0C0C0",
    "dateTimeColor": "#000000",
    "teamIcon": "https://example.com/icon2.png"
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

## 5. Bulk Add Bingo Webhooks

Adds multiple Discord webhooks for characters.

- **Method**: `POST`
- **URL**: `/api/BingoConfig/webhooks/bulk`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
[
  {
    "characterName": "PlayerOne",
    "webhookUrl": "https://discord.com/api/webhooks/..."
  },
  {
    "characterName": "PlayerTwo",
    "webhookUrl": "https://discord.com/api/webhooks/..."
  }
]
```
- **Description**: Adds multiple new records to the webhooks table.

---

## 6. Clone Bingo Configuration

Clones team settings and webhooks from one character to another.

- **Method**: `POST`
- **URL**: `/api/BingoConfig/clone`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "sourceCharacterName": "PlayerOne",
  "targetCharacterName": "PlayerTwo"
}
```
- **Description**: Copies the team name, team color, date/time color, and team icon from the source character to the target character. It also copies all Discord webhooks registered for the source character to the target character. If the target character already has a team config, it will be updated.

---

## 7. Bulk Clone Bingo Configurations

Clones team settings and webhooks for multiple pairs of characters at once.

- **Method**: `POST`
- **URL**: `/api/BingoConfig/clone/bulk`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
[
  {
    "sourceCharacterName": "PlayerOne",
    "targetCharacterName": "PlayerTwo"
  },
  {
    "sourceCharacterName": "PlayerThree",
    "targetCharacterName": "PlayerFour"
  }
]
```
- **Description**: Performs bulk cloning of team settings (including team name, color, and icon) and webhooks. For each pair in the list, it copies the configuration from the source character to the target character.

---

## 8. Get Bingo Configuration

Fetches all configuration (webhooks, items, and team settings) for a specific character.

- **Method**: `POST`
- **URL**: `/api/BingoConfig?character={CHARACTER_NAME}`
- **Body**: (Optional empty object `{}`)
- **Description**: Returns the current configuration including all registered webhooks for the character, all items in the bingo pool, and the team configuration if it exists.
- **Response**:
```json
{
  "webhooks": ["https://discord.com/api/webhooks/..."],
  "items": [
    {
      "name": "Twisted bow",
      "source": "Chambers of Xeric"
    }
  ],
  "teamConfig": {
    "teamName": "The Champions",
    "teamNameColor": "#FFD700",
    "dateTimeColor": "#FFFFFF",
    "teamIcon": "https://example.com/icon.png"
  }
}
```

---

## 9. Delete Bingo Configuration

Deletes any team configurations and webhooks for a specific character name.

- **Method**: `DELETE`
- **URL**: `/api/BingoConfig?character={CHARACTER_NAME}`
- **Description**: Deletes all `BingoTeamConfig` and `BingoWebhook` records associated with the given character (case-insensitive). Global `BingoItem` entries are not affected.

---

## 10. Ingest Logs

Ingests a list of loot/activity logs for characters.

- **Method**: `POST`
- **URL**: `/api/Logs`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
[
  {
    "player": "PlayerOne",
    "type": "LOOT",
    "timestamp": 1677456000,
    "data": {
      "source": "Chambers of Xeric",
      "items": [
        {
          "id": 20997,
          "name": "Twisted bow",
          "quantity": 1,
          "price": 1200000000
        }
      ],
      "totalValue": 1200000000,
      "kc": 100
    }
  }
]
```
- **Description**: Accepts a list of logs. Each log includes the player name, log type (e.g., "LOOT"), a unix timestamp, and type-specific data.

---

## 11. Ingest Death

Ingests a death record for a character.

- **Method**: `POST`
- **URL**: `/api/Deaths`
- **Headers**: `Content-Type: application/json`
- **Body**:
```json
{
  "player": "PlayerOne",
  "type": "DEATH",
  "timestamp": 1677456000,
  "data": {
    "regionId": 12345,
    "killer": "Zulrah"
  }
}
```
- **Description**: Accepts a single death record including the player name, type ("DEATH"), unix timestamp, and death-specific data like region ID and killer name.

---

## 12. Get Bingo Team Icons

Returns a list of predefined team names and their corresponding icons.

- **Method**: `GET`
- **URL**: `/api/BingoConfig/teamIcons`
- **Description**: Returns a static list of objects, each containing a `teamName` and a `teamIcon` string. This list is currently hardcoded and used by the client plugin.
- **Response**:
```json
[
  {
    "teamName": "Shayzien Shower Skippers",
    "teamIcon": "https://www.emoji.family/api/emojis/1f9a8/twemoji/png/32"
  }
]
```

---

## 13. Get All Bingo Team Mappings

Returns a list of all characters and their assigned team names.

- **Method**: `GET`
- **URL**: `/api/BingoConfig/teams`
- **Description**: Returns an array of objects, each containing a `character` and a `teamName`. This is used to map specific character names in game to their respective team based on the configurations stored in the database.
- **Response**:
```json
[
  {
    "character": "PlayerOne",
    "teamName": "The Champions"
  },
  {
    "character": "PlayerTwo",
    "teamName": "The Challengers"
  }
]
```

---

## 14. Player Loot Value Leaderboard Report

Generates a CSV report showing total loot value per player within a specified timeframe, deduplicating across `LOOT`, `RAID_LOOT`, and `VALUABLE_DROP` log types.

- **Method**: `GET`
- **URL**: `/api/reporting/leaderboard/player-loot-value?from={FROM_DATETIME}&to={TO_DATETIME}`
- **Query Parameters**:
    - `from` (required): `DateTimeOffset` (ISO 8601 format, e.g., `2026-02-26T23:00:00+00:00`)
    - `to` (required): `DateTimeOffset` (ISO 8601 format, e.g., `2026-03-09T01:00:00+00:00`)
- **Description**: Returns a CSV file named `player_loot_leaderboard.csv` containing the total loot value for each player, sorted by `total_loot_value` in descending order.
- **Response**: CSV with columns: `character_name,team_name,total_loot_value`

---

## 15. Team Loot Value Leaderboard Report

Generates a CSV report showing total loot value per team within a specified timeframe, deduplicating across `LOOT`, `RAID_LOOT`, and `VALUABLE_DROP` log types.

- **Method**: `GET`
- **URL**: `/api/reporting/leaderboard/team-loot-value?from={FROM_DATETIME}&to={TO_DATETIME}`
- **Query Parameters**:
    - `from` (required): `DateTimeOffset` (ISO 8601 format, e.g., `2026-02-26T23:00:00+00:00`)
    - `to` (required): `DateTimeOffset` (ISO 8601 format, e.g., `2026-03-09T01:00:00+00:00`)
- **Description**: Returns a CSV file named `team_loot_leaderboard.csv` containing the total loot value for each team, sorted by `total_loot_value` in descending order. Players without a team are grouped under an empty string team name.
- **Response**: CSV with columns: `team_name,total_loot_value`
