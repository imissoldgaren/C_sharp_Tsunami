using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myLibrary
{
    public interface ISetup
    {
        // Gets the water height at a given point.
        float GetHeight(float x, float y);

        // Gets the momentum in x-direction.
        float GetMomentumX(float x, float y);

        // Gets the momentum in y-direction.
        float GetMomentumY(float x, float y);

        // Gets the bathymetry at a given point.
        float GetBathymetry(float x, float y);
    }
}
