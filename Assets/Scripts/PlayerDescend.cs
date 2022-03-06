using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDescend : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D PlayerCollider;

    [Header("Descend")]
    [SerializeField] float PlatformCheckRadius;
    [SerializeField] Transform Feet;
    [SerializeField] private LayerMask PlatformLayer;

    private Collider2D _Platform;
    private bool _Descend;

    private List<Collider2D> _LeftPlatforms = new List<Collider2D>();

    private void Update()
    {
        _Descend = Input.GetButtonDown("Descend") ? true : _Descend;
    }

    private void FixedUpdate()
    {
        _Platform = Physics2D.OverlapCircle(Feet.position, PlatformCheckRadius, PlatformLayer);

        if (_Descend && _Platform)
        {
            Physics2D.IgnoreCollision(PlayerCollider, _Platform, true);
            _LeftPlatforms.Add(_Platform);
        }

        _Descend = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var platform in _LeftPlatforms)
        {
            if (platform == collision)
            {
                Physics2D.IgnoreCollision(PlayerCollider, platform, false);
            }
        }
    }
}
