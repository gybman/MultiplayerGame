using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class PlayerVisual : NetworkBehaviour
{
    private readonly NetworkVariable<Color> _netColor = new();
    private readonly Color[] _colors = { Color.red, Color.blue, Color.green, Color.yellow };    // allows different colors for players who join the game
    private int _index;

    [SerializeField] private MeshRenderer _renderer;

    private void Awake()
    {
        // subscribing to a change event. this is how the owner will change it's color
        // could also be used for future color changes
        _netColor.OnValueChanged += OnValueChanged;
    }

    public override void OnDestroy() => _netColor.OnValueChanged -= OnValueChanged;

    private void OnValueChanged(Color prev, Color next) => _renderer.material.color = next;

    public override void OnNetworkSpawn()
    {
        // RPCs are queued up to run.
        // if we tried to immediately set our color locally after calling this rpc is wouldn't have propagated
        if (IsOwner)
        {
            _index = (int)OwnerClientId;
            CommitNetworkColorServerRpc(GetNextColor());
        }
        else
        {
            _renderer.material.color = _netColor.Value;
        }
    }

    [ServerRpc]
    private void CommitNetworkColorServerRpc(Color color) => _netColor.Value = color;

    private Color GetNextColor() => _colors[_index++ % _colors.Length];
}
