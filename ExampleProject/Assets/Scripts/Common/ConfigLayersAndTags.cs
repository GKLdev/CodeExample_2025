using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigLayersAndTags", menuName = "Configs/Common/ConfigLayersAndTags")]
public class ConfigLayersAndTags : ScriptableObject
{
    public string Tag_KillDomain = "KillDomain";

    public LayerMask Layer_hitbox;
}
