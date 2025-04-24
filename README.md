# Music Discovery API & Data Pipeline

## Project Summary

This project showcases an **ASP.NET Core Web API** designed for jazz music discovery. Its core function is to build and maintain a **custom, enriched music database**, reducing reliance on the long-term availability of third-party APIs. It features a **data ingestion pipeline** that integrates data from Spotify, enhances it using OpenAI (GPT-4o), retrieves streaming links via Song.link and stores the consolidated information in a PostgreSQL database managed by Entity Framework Core.

## Core Features & Capabilities

* **Automated Data Ingestion:** Populates the database with artist discographies via an admin endpoint.
* **AI-Enhanced Content:** Leverages OpenAI (GPT-4o) to generate rich metadata (descriptions, themes, moods, subgenres, artist bios) beyond standard API offerings.
* **Persistent & Resilient Datastore:** Creates an independent database of music information, ensuring data longevity.
* **Relative Popularity Scoring:** Implements percentile-based scoring for albums and artists, offering context-aware popularity metrics.
* **Streaming Link Aggregation:** Fetches and stores unique identifiers for albums across multiple streaming platforms (Spotify, YouTube, Apple Music, etc.).
* **RESTful API Access:** Provides endpoints to query detailed album and artist information from the custom database.

## Technology Stack

* **Backend:** ASP.NET Core ([Specify .NET Version, e.g., 8.0]), C#
* **Database:** PostgreSQL
* **ORM:** Entity Framework Core (Code-First)
* **APIs Integrated:** Spotify, OpenAI (GPT-4o), Song.link
* **Key Libraries:** OpenAI SDK, Newtonsoft.Json, Serilog

## Data Ingestion Pipeline

This project implements a multi-stage pipeline to build the database:

1.  **Spotify API:** Fetches initial album and artist details (metadata, popularity, Spotify IDs).
2.  **OpenAI (GPT-4o):** Receives Spotify data; returns enriched information like detailed descriptions, inferred genres/subgenres/moods, themes, and artist biographies/instruments based on tailored prompts.
3.  **Song.link API:** Uses Spotify Album IDs to retrieve unique identifiers for the album on various other streaming platforms.
4.  **PostgreSQL Database:** Stores the combined, cleaned, and structured data from all sources, managed via EF Core.

This pipeline orchestrates multiple external services, processes data transformations and loads results into a structured datastore reliably.

## Database Schema & Design

A relational PostgreSQL schema was designed using EF Core (code-first) with snake\_case naming conventions. The goal was to create a normalized and queryable structure for the aggregated music data.

**Key Tables:**

* `albums`: Core album details, including Spotify/GPT/SongLink fields.
* `artists`: Artist details, including Spotify/GPT fields.
* `genres`: Primary music genres (e.g., Jazz).
* `subgenres`: Specific subgenres linked to a parent `Genre`.
* `moods`: Descriptive moods associated with albums.
* `disco_transactions`: Tracks each data ingestion run (e.g., processing one artist's discography).
* `album_artists` (Junction): Many-to-Many between `albums` and `artists`.
* `album_subgenres` (Junction): Many-to-Many between `albums` and `subgenres`.
* `album_moods` (Junction): Many-to-Many between `albums` and `moods`.

**Relationships & Data Integrity:**

* Standard one-to-many (e.g., `genres` to `albums`, `genres` to `subgenres`) and many-to-many relationships are established using junction tables.
* Crucially, every entity created or modified during a specific data ingestion run (`albums`, `artists`, `genres`, etc., including junction table entries) holds a foreign key relationship to the `disco_transactions` table via a unique `disco_transaction_id` (GUID). This ensures **data provenance** and traceability for each batch operation.

## API Endpoints

*(Note: Base URL typically `http://localhost:PORT` or `https://localhost:PORT`)*

**Admin Endpoints (`/api/admin`)**

* `POST /create-discography`: Initiates the data pipeline for a given artist name (passed as JSON string in body) to populate the database.
* `POST /normalize-album-scores`: Calculates and updates percentile popularity scores for all albums in the database.
* `POST /normalize-artist-scores`: Calculates and updates percentile popularity scores for all artists in the database.
* `POST /test?spotifyAlbumId={id}`: (Test Utility) Fetches streaming links for a specific Spotify album ID via Song.link.

**Public Endpoints (`/api/album`, `/api/artist`)**

* `GET /album/{Id}`: Retrieves detailed information for a specific album by its database ID.
* `GET /album/random`: Retrieves detailed information for a randomly selected album.
* `GET /artist/get-artist?artistId={id}`: Retrieves detailed artist information (including notable albums) by database ID.

## Technical Highlights & Problem Solving

* **Data Provenance and Management:** To manage data ingested in batches (e.g., one artist's discography), a `disco_transactions` table was implemented. Every record created across multiple tables during a single operation shares a unique GUID (`disco_transaction_id`). This design provides clear data lineage and simplifies potential future operations like targeted data analysis or rollback per transaction.
* **Contextual Popularity Metric:** Recognizing that Spotify's absolute popularity scores can be misleading for niche genres (e.g., Jazz albums compared to mainstream pop), an endpoint (`POST /api/admin/normalize-album-scores`) was created. This calculates and stores a *percentile score* for each album, reflecting its popularity *relative* to other albums within the database. This provides a more meaningful comparative metric within the application's specific domain.

## Setup & Usage

1.  **Prerequisites:** .NET SDK, PostgreSQL, Git.
2.  **Clone:** `git clone [your-repository-url]`
3.  **Configure:** Set API Keys (OpenAI, Spotify) and PostgreSQL connection string using .NET User Secrets:
    ```bash
    cd [project-directory]
    dotnet user-secrets init
    dotnet user-secrets set "openai" "YOUR_KEY"
    dotnet user-secrets set "spotifyClientId" "YOUR_ID"
    dotnet user-secrets set "spotifyClientSecret" "YOUR_SECRET"
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_DB_CONNECTION_STRING"
    ```
4.  **Database:** Ensure PostgreSQL server is running. Apply EF Core migrations:
    ```bash
    dotnet ef database update
    ```
5.  **Run:**
    ```bash
    dotnet run
    ```
    The API will start, listening on specified ports (see console output).

---
