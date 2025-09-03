using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "disco_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    time_stamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    request_details = table.Column<string>(type: "TEXT", nullable: true),
                    response_status_code = table.Column<int>(type: "INTEGER", nullable: true),
                    error_message = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_disco_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genre_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genre_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jazz_era_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    start_year = table.Column<int>(type: "INTEGER", nullable: true),
                    end_year = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jazz_era_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    release_date = table.Column<string>(type: "TEXT", nullable: false),
                    release_date_precision = table.Column<string>(type: "TEXT", nullable: false),
                    total_tracks = table.Column<int>(type: "INTEGER", nullable: true),
                    image_url = table.Column<string>(type: "TEXT", nullable: false),
                    spotify_popularity = table.Column<int>(type: "INTEGER", nullable: false),
                    label = table.Column<string>(type: "TEXT", nullable: true),
                    additional_artists = table.Column<string>(type: "TEXT", nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    is_original_release = table.Column<bool>(type: "INTEGER", nullable: true),
                    spotify_id = table.Column<string>(type: "TEXT", nullable: false),
                    youtube_id = table.Column<string>(type: "TEXT", nullable: true),
                    apple_music_id = table.Column<string>(type: "TEXT", nullable: true),
                    amazon_music_id = table.Column<string>(type: "TEXT", nullable: true),
                    pandora_id = table.Column<string>(type: "TEXT", nullable: true),
                    sortable_date = table.Column<int>(type: "INTEGER", nullable: true),
                    popularity_rating = table.Column<int>(type: "INTEGER", nullable: true),
                    average_emotional_tone = table.Column<int>(type: "INTEGER", nullable: true),
                    average_energy_level = table.Column<int>(type: "INTEGER", nullable: true),
                    disco_transaction_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums", x => x.id);
                    table.ForeignKey(
                        name: "fk_albums_disco_transaction_disco_transaction_id",
                        column: x => x.disco_transaction_id,
                        principalTable: "disco_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artists",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    genres = table.Column<string>(type: "TEXT", nullable: true),
                    image_url = table.Column<string>(type: "TEXT", nullable: true),
                    spotify_popularity = table.Column<int>(type: "INTEGER", nullable: false),
                    spotify_id = table.Column<string>(type: "TEXT", nullable: false),
                    birth_year = table.Column<string>(type: "TEXT", nullable: true),
                    death_year = table.Column<string>(type: "TEXT", nullable: true),
                    biography = table.Column<string>(type: "TEXT", nullable: true),
                    instrument = table.Column<string>(type: "TEXT", nullable: true),
                    related_artists = table.Column<string>(type: "TEXT", nullable: false),
                    influences = table.Column<string>(type: "TEXT", nullable: false),
                    popularity_rating = table.Column<int>(type: "INTEGER", nullable: true),
                    album_count = table.Column<int>(type: "INTEGER", nullable: false),
                    average_album_popularity_rating = table.Column<int>(type: "INTEGER", nullable: true),
                    average_emotional_tone = table.Column<int>(type: "INTEGER", nullable: true),
                    average_energy_level = table.Column<int>(type: "INTEGER", nullable: true),
                    subgenre_breakdown = table.Column<string>(type: "TEXT", nullable: true),
                    years_active = table.Column<string>(type: "TEXT", nullable: true),
                    mood_breakdown = table.Column<string>(type: "TEXT", nullable: true),
                    disco_transaction_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artists", x => x.id);
                    table.ForeignKey(
                        name: "fk_artists_disco_transaction_disco_transaction_id",
                        column: x => x.disco_transaction_id,
                        principalTable: "disco_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "moods",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    emotional_tone = table.Column<int>(type: "INTEGER", nullable: false),
                    energy_level = table.Column<int>(type: "INTEGER", nullable: false),
                    disco_transaction_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_moods", x => x.id);
                    table.ForeignKey(
                        name: "fk_moods_disco_transaction_disco_transaction_id",
                        column: x => x.disco_transaction_id,
                        principalTable: "disco_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subgenres",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    genre_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    disco_transaction_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subgenres", x => x.id);
                    table.ForeignKey(
                        name: "fk_subgenres_disco_transaction_disco_transaction_id",
                        column: x => x.disco_transaction_id,
                        principalTable: "disco_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_subgenres_genre_types_genre_type_id",
                        column: x => x.genre_type_id,
                        principalTable: "genre_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_genres",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "INTEGER", nullable: false),
                    genre_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    disco_transaction_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_genres", x => new { x.album_id, x.genre_type_id });
                    table.ForeignKey(
                        name: "fk_album_genres_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_genres_disco_transactions_disco_transaction_id",
                        column: x => x.disco_transaction_id,
                        principalTable: "disco_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_genres_genre_types_genre_type_id",
                        column: x => x.genre_type_id,
                        principalTable: "genre_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_jazz_eras",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "INTEGER", nullable: false),
                    jazz_era_type_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_jazz_eras", x => new { x.album_id, x.jazz_era_type_id });
                    table.ForeignKey(
                        name: "fk_album_jazz_eras_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_jazz_eras_jazz_era_types_jazz_era_type_id",
                        column: x => x.jazz_era_type_id,
                        principalTable: "jazz_era_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_artists",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "INTEGER", nullable: false),
                    artist_id = table.Column<int>(type: "INTEGER", nullable: false),
                    original_album_order = table.Column<int>(type: "INTEGER", nullable: true),
                    disco_transaction_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_artists", x => new { x.album_id, x.artist_id });
                    table.ForeignKey(
                        name: "fk_album_artists_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_artists_artists_artist_id",
                        column: x => x.artist_id,
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_artists_disco_transactions_disco_transaction_id",
                        column: x => x.disco_transaction_id,
                        principalTable: "disco_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_moods",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "INTEGER", nullable: false),
                    mood_id = table.Column<int>(type: "INTEGER", nullable: false),
                    disco_transaction_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_moods", x => new { x.album_id, x.mood_id });
                    table.ForeignKey(
                        name: "fk_album_moods_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_moods_disco_transactions_disco_transaction_id",
                        column: x => x.disco_transaction_id,
                        principalTable: "disco_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_moods_moods_mood_id",
                        column: x => x.mood_id,
                        principalTable: "moods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_subgenres",
                columns: table => new
                {
                    album_id = table.Column<int>(type: "INTEGER", nullable: false),
                    subgenre_id = table.Column<int>(type: "INTEGER", nullable: false),
                    disco_transaction_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_album_subgenres", x => new { x.album_id, x.subgenre_id });
                    table.ForeignKey(
                        name: "fk_album_subgenres_albums_album_id",
                        column: x => x.album_id,
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_subgenres_disco_transactions_disco_transaction_id",
                        column: x => x.disco_transaction_id,
                        principalTable: "disco_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_album_subgenres_subgenres_subgenre_id",
                        column: x => x.subgenre_id,
                        principalTable: "subgenres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "genre_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Classical" },
                    { 2, "Jazz" },
                    { 3, "Blues" },
                    { 4, "Funk" },
                    { 5, "Rock" },
                    { 6, "Pop" },
                    { 7, "Country" },
                    { 8, "Folk" },
                    { 9, "Hip-Hop/Rap" },
                    { 10, "Electronic" },
                    { 11, "Reggae" },
                    { 12, "R&B/Soul" },
                    { 13, "Metal" },
                    { 14, "World Music" },
                    { 15, "Gospel" },
                    { 16, "Latin" }
                });

            migrationBuilder.InsertData(
                table: "jazz_era_types",
                columns: new[] { "id", "end_year", "name", "start_year" },
                values: new object[,]
                {
                    { 1, 1918, "Ragtime", 1897 },
                    { 2, 1929, "Early Jazz / New Orleans Jazz", 1910 },
                    { 3, 1942, "Kansas City Jazz", 1928 },
                    { 4, 1945, "Swing Era / Big Band Era", 1930 },
                    { 5, 1953, "Gypsy Jazz (Jazz Manouche)", 1934 },
                    { 6, 1955, "Bebop", 1945 },
                    { 7, 1959, "Cool Jazz", 1949 },
                    { 8, 1965, "Hard Bop", 1954 },
                    { 9, 1969, "Soul Jazz", 1958 },
                    { 10, 1967, "Modal Jazz", 1958 },
                    { 11, 1974, "Free Jazz / Avant-Garde", 1960 },
                    { 12, 1972, "Post-Bop", 1964 },
                    { 13, 1989, "Jazz Fusion", 1969 },
                    { 14, 1982, "Jazz-Funk", 1972 },
                    { 15, null, "Latin Jazz (Afro-Cuban & Brazilian)", 1947 },
                    { 16, 1999, "Neo-Bop", 1980 },
                    { 17, 1999, "Smooth Jazz", 1982 },
                    { 18, 1997, "Acid Jazz", 1987 },
                    { 19, null, "Contemporary Jazz / Modern Creative", 2000 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_album_artists_artist_id",
                table: "album_artists",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_artists_disco_transaction_id",
                table: "album_artists",
                column: "disco_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_genres_disco_transaction_id",
                table: "album_genres",
                column: "disco_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_genres_genre_type_id",
                table: "album_genres",
                column: "genre_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_jazz_eras_jazz_era_type_id",
                table: "album_jazz_eras",
                column: "jazz_era_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_moods_disco_transaction_id",
                table: "album_moods",
                column: "disco_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_moods_mood_id",
                table: "album_moods",
                column: "mood_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_subgenres_disco_transaction_id",
                table: "album_subgenres",
                column: "disco_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_album_subgenres_subgenre_id",
                table: "album_subgenres",
                column: "subgenre_id");

            migrationBuilder.CreateIndex(
                name: "ix_albums_disco_transaction_id",
                table: "albums",
                column: "disco_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_artists_disco_transaction_id",
                table: "artists",
                column: "disco_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_moods_disco_transaction_id",
                table: "moods",
                column: "disco_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_subgenres_disco_transaction_id",
                table: "subgenres",
                column: "disco_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_subgenres_genre_type_id",
                table: "subgenres",
                column: "genre_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "album_artists");

            migrationBuilder.DropTable(
                name: "album_genres");

            migrationBuilder.DropTable(
                name: "album_jazz_eras");

            migrationBuilder.DropTable(
                name: "album_moods");

            migrationBuilder.DropTable(
                name: "album_subgenres");

            migrationBuilder.DropTable(
                name: "artists");

            migrationBuilder.DropTable(
                name: "jazz_era_types");

            migrationBuilder.DropTable(
                name: "moods");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropTable(
                name: "subgenres");

            migrationBuilder.DropTable(
                name: "disco_transactions");

            migrationBuilder.DropTable(
                name: "genre_types");
        }
    }
}
