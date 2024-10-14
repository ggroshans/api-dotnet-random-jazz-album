# random-album-api

Current JSON for Album seeding
```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "properties": {
    "id": { "type": "integer" }, // unique identifier for the album
    "title": { "type": "string" }, // album title
    "artist_name": { "type": "string" }, // artist or band name (to be added later)
    "release_date": { "type": "string", "format": "date" }, // release date (YYYY-MM-DD)
    "genre": { "type": "string" }, // main genre
    "sub_genres": { "type": "array", "items": { "type": "string" } }, // list of subgenres
    "cover_image_url": { "type": "string", "format": "uri" }, // URL of the album cover image
    "total_tracks": { "type": "integer" }, // total number of tracks
    "label": { "type": "string" }, // record label (optional)
    "mood": { "type": "array", "items": { "type": "string" } }, // overall mood(s) of the album
    "featured_artists": { "type": "array", "items": { "type": "string" } }, // list of featured artists
    "recording_location": { "type": "string" }, // location where album was recorded
    "length": { "type": "integer" }, // total length of the album in minutes
    "release_notes": { "type": "string" }, // notes from the artist (optional)
    "album_theme": { "type": "string" }, // main theme or concept of the album
    "fan_rating": { "type": "number" }, // average fan rating (e.g., 4.5)
    "story_behind_title": { "type": "string" }, // explanation of the album title
    "album_position": { "type": "integer" }, // album's position in the artist’s discography (e.g., 1st, 2nd)
    "popular_tracks": { "type": "array", "items": { "type": "string" } }, // list of popular tracks
    "awards": { "type": "array", "items": { "type": "string" } }, // list of awards in "award (year)" format chronologically
    "cover_art_description": { "type": "string" }, // brief description of the album cover’s design
    "personal_anecdotes": { "type": "string" }, // notes from the artist or producers
    "setlist_context": { "type": "string" }, // context on how tracks fit into live setlists
    "created_at": { "type": "string", "format": "date-time" }, // creation timestamp (ISO 8601)
    "updated_at": { "type": "string", "format": "date-time" } // last update timestamp (ISO 8601)
  },
  "required": ["id", "title", "artist_name", "release_date", "genre", "cover_image_url", "total_tracks", "created_at", "updated_at"]
}

