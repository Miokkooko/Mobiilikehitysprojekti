using UnityEngine;

public class ShitTesting : MonoBehaviour
{
    public StatusEffect effect;
    public void AddEffectToUnit(Unit target)
    {
        Unit.ApplyStatusEffect(effect, target);
    }
}
