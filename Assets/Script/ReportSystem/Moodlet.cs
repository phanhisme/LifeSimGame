using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Moodlet", menuName = "Moodlet")]
public class Moodlet : ScriptableObject
{
    public int moodletID;

    public string moodletName;

    public Sprite moodletSprite;
    public string reason; //reason why this moodlet appear

    public int  secondsDuration;
    public int effectPercentage;

    public int timeToActivateMoodlet;

    public enum EffectType {POSITIVE, NEGATIVE, NONE, INFLUENCE }
    public EffectType effectType;
    public EffectType secondEffect;
}
