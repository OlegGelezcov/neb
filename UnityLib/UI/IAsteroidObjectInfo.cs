using System.Collections.Generic;
using UnityEngine;

namespace Nebula.UI {
    public interface IAsteroidObjectInfo : IIconObjectInfo {
        List<Sprite> ContentSprites { get; }
    }
}
