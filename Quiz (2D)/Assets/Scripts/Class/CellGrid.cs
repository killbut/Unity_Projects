using System;
using UnityEngine;

    [Serializable]
    public class CellGrid
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public GameObject Value { get; set; }
        public GameObject Background { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
