﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EriverTrackers
{
    class XConfSettings
    {
        public double Heigth { get; set; }
        public double Width { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
        public double Dz { get; set; }
        public double Dangle { get; set; }

        public XConfSettings(int Heigth, int Width, int Dx, int Dy, int Dz, int Dangle)
        {
            this.Heigth = Heigth;
            this.Width = Width;
            this.Dx = Dx;
            this.Dy = Dy;
            this.Dz = Dz;
            this.Dangle = Dangle;
        }
    }
}
