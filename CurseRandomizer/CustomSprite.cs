using KorzUtils.Helper;
using ItemChanger;
using System;
using UnityEngine;

namespace CurseRandomizer;

[Serializable]
internal class CustomSprite : ISprite
{
    public CustomSprite() { }

    public CustomSprite(string key)
    {
        if (!string.IsNullOrEmpty(key))
            Key = key;
    }

    public string Key { get; set; } = "Fool";

    [Newtonsoft.Json.JsonIgnore]
    public Sprite Value => SpriteHelper.CreateSprite<CurseRandomizer>(Key);

    public ISprite Clone() => new CustomSprite(Key);
}