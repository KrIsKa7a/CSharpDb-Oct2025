namespace MusicHub.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class AlbumViewModel
    {
        public string Name { get; set; } = null!;

        public DateTime ReleaseDate { get; set; }

        public string ProducerName { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public IEnumerable<SongViewModel> AlbumSongs { get; set; }
            = new List<SongViewModel>();
    }
}
