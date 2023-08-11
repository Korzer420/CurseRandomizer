using KorzUtils.Helper;
using ItemChanger;
using System;
using UnityEngine;

namespace CurseRandomizer;

[Serializable]
public class CustomSprite : ISprite
{
    public CustomSprite() { }

    public CustomSprite(string key)
    {
        if (!string.IsNullOrEmpty(key))
            Key = key;
    }

    public string Key { get; set; } = "Fool";

    [Newtonsoft.Json.JsonIgnore]
    public Sprite Value => SpriteHelper.CreateSprite<CurseRandomizer>("Sprites."+Key);

    public ISprite Clone() => new CustomSprite(Key);
}