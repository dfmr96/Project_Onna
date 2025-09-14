using Player;
using System;
using System.Collections.Generic;

public class NewMutationEffect
{
    private Dictionary<(MutationType, SystemType, SlotType), Action<PlayerModel>> effects;
    
    public NewMutationEffect()
    {
        effects = new Dictionary<(MutationType, SystemType, SlotType), Action<PlayerModel>>();

        // Gamma integumentary example
        //effects[(MutationType.Gamma, SystemType.Integumentary, SlotType.Major)] =
        //    (player) => player.ALGOALPLAYER();

        //effects[(MutationType.Gamma, SystemType.Integumentary, SlotType.Minor)] =
        //    (player) => player.ALGOALPLAYER();

        // Gamma nerve example
        //effects[(MutationType.Gamma, SystemType.Nerve, SlotType.Major)] =
        //    (player) => player.ALGOALPLAYER();

        //effects[(MutationType.Gamma, SystemType.Nerve, SlotType.Minor)] =
        //    (player) => player.ALGOALPLAYER();
    }

    public void ApplyEffect(NewMutations mutation, PlayerModel player, SystemType system, SlotType slot)
    {
        var key = (mutation.Type, system, slot);
        if (effects.TryGetValue(key, out var effect)) effect(player);
    }
}
