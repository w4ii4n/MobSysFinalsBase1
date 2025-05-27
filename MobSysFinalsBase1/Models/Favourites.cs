using SQLite;
using System;

namespace MobSysFinalsBase1.Models
{
    public class Favourite
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        public int UserID { get; set; }
        [NotNull]
        public string Title { get; set; }
        [NotNull]
        public string Author { get; set; }
        public string PublishedDate { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string AmazonLink { get; set; }
        public string Thumbnail { get; set; }
        public string SmallThumbnail { get; set; }
        public DateTime AddedDate { get; set; }
    }
}