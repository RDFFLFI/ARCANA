﻿using RDF.Arcana.API.Common;

namespace RDF.Arcana.API.Domain
{
    public class Terms : BaseEntity
    {
        public string Term { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public int AddedBy { get; set; }


        public User AddedByUser { get; set; }
    }
}