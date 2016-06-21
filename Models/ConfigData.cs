﻿namespace imgmrg.Models
{
    public class ConfigData
    {
        public Transformation[] transformations { get; set; }
    }
    public class Transformation
    {
        public string input_rgb { get; set; }
        public string input_a { get; set; }
        public string output { get; set; }
    }
}