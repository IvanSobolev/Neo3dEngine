using _3dEngine.Interfaces.modifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3dEngine.Interfaces;
public interface IDisplaysManagerAsync
{
    RenderData FindClosestIntersection(Ray ray, List<IDisplays> displays);
}
