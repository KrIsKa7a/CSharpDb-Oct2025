namespace MusicHub
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    using Microsoft.EntityFrameworkCore;

    using Data;
    using Initializer;
    using ViewModels;

    public class StartUp
    {
        private static readonly Func<MusicHubDbContext, int, IEnumerable<AlbumViewModel>> compiledAlbumQuery =
            EF.CompileQuery((MusicHubDbContext context, int producerId) =>
                context.Albums
                    .Where(a => a.ProducerId == producerId)
                    .Select(a => new AlbumViewModel
                    {
                        Name = a.Name,
                        ReleaseDate = a.ReleaseDate,
                        ProducerName = a.Producer!.Name,
                        AlbumSongs = a.Songs
                            .Select(s => new SongViewModel
                            {
                                Name = s.Name,
                                Price = s.Price,
                                WriterName = s.Writer.Name
                            })
                            .OrderByDescending(s => s.Name)
                            .ThenBy(s => s.WriterName)
                            .ToArray(),
                        TotalPrice = a.Price,
                    }));

        public static void Main()
        {
            using MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            string result = ExportAlbumsInfo(context, 9);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds); // 308ms

            sw.Restart();

            result = ExportAlbumsInfo_Compiled(context, 9);

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds); // 27ms ~ 15x times faster (EF Core Cache + PreCompile)
                                                       // 280ms ~ 38ms faster (Without EF Core Cache + PreCompile)
        }

        // Problem 02
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            // Always prefer to use parameterized queries to avoid SQL Injection attacks and increase EF Cache performance during SQL generation
            // LINQ using Constant value can't make use of the EF Core Cache and disturb the Cache Memory -> SQL Generation Overhead
            var producerAlbums = context.Albums
                .Where(a => a.ProducerId == producerId)
                .Select(a => new
                {
                    a.Name,
                    a.ReleaseDate,
                    ProducerName = a.Producer!.Name,
                    AlbumSongs = a.Songs
                        .Select(s => new
                        {
                            s.Name,
                            s.Price,
                            WriterName = s.Writer.Name
                        })
                        .OrderByDescending(s => s.Name)
                        .ThenBy(s => s.WriterName)
                        .ToArray(),
                    TotalPrice = a.Price,
                })
                .ToArray()
                .OrderByDescending(a => a.TotalPrice)
                .ToArray();

            foreach (var album in producerAlbums)
            {
                sb
                    .AppendLine($"-AlbumName: {album.Name}")
                    .AppendLine(
                        $"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine($"-Songs:");

                int songNumber = 1;
                foreach (var song in album.AlbumSongs)
                {
                    sb
                        .AppendLine($"---#{songNumber++}")
                        .AppendLine($"---SongName: {song.Name}")
                        .AppendLine($"---Price: {song.Price.ToString("f2")}")
                        .AppendLine($"---Writer: {song.WriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalPrice.ToString("f2")}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportAlbumsInfo_Compiled(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            // Always prefer to use parameterized queries to avoid SQL Injection attacks and increase EF Cache performance during SQL generation
            // LINQ using Constant value can't make use of the EF Core Cache and disturb the Cache Memory -> SQL Generation Overhead
            var producerAlbums = compiledAlbumQuery(context, producerId)
                .ToArray()
                .OrderByDescending(a => a.TotalPrice)
                .ToArray();

            foreach (var album in producerAlbums)
            {
                sb
                    .AppendLine($"-AlbumName: {album.Name}")
                    .AppendLine(
                        $"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine($"-Songs:");

                int songNumber = 1;
                foreach (var song in album.AlbumSongs)
                {
                    sb
                        .AppendLine($"---#{songNumber++}")
                        .AppendLine($"---SongName: {song.Name}")
                        .AppendLine($"---Price: {song.Price.ToString("f2")}")
                        .AppendLine($"---Writer: {song.WriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalPrice.ToString("f2")}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context
                .Songs
                .Select(s => new
                {
                    s.Name,
                    PerformersNames = s.SongPerformers
                        .Select(sp => new
                        {
                            sp.Performer.FirstName,
                            sp.Performer.LastName,
                        })
                        .OrderBy(sp => sp.FirstName)
                        .ThenBy(sp => sp.LastName)
                        .ToArray(),
                    WriterName = s.Writer.Name,
                    AlbumProducerName =
                        s.Album != null ? (s.Album.Producer != null ? s.Album.Producer.Name : null) : null,
                    s.Duration,
                })
                .OrderBy(s => s.Name)
                .ThenBy(s => s.WriterName)
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration) // In-memory filtering due to limitations in EntityFrameworkCore.SqlServer DbProvider... Fix ASAP
                .ToArray();

            int songNumber = 1;
            foreach (var song in songs)
            {
                sb
                    .AppendLine($"-Song #{songNumber++}")
                    .AppendLine($"---SongName: {song.Name}")
                    .AppendLine($"---Writer: {song.WriterName}");
                foreach (var performer in song.PerformersNames)
                {
                    sb
                        .AppendLine($"---Performer: {performer.FirstName} {performer.LastName}");
                }

                sb
                    .AppendLine($"---AlbumProducer: {song.AlbumProducerName}")
                    .AppendLine($"---Duration: {song.Duration.ToString("c")}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
