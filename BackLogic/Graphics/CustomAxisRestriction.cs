using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.ViewportConstraints;

namespace BackLogic.Graphics
{
    public class CustomAxisRestriction : ViewportConstraint
    {
        private double xMin;
        private double yMin;
        private double Height;
        private double Width;
        public CustomAxisRestriction(double xMin, double yMin, double Height, double Width)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.Height = Height;
            this.Width = Width;
        }
        public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
        {
            proposedDataRect.XMin = xMin;
            proposedDataRect.YMin = yMin;
            proposedDataRect.Width = Width;
            proposedDataRect.Height = Height;
            return proposedDataRect;
        }
    }

}
