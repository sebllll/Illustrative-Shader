using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MobRules.Interfaces
{
    interface ICameraService
    {
        Matrix View
        {
            get;
        }

        Matrix Projection
        {
            get;
        }
    }
}
