using Unity.Netcode;

namespace GameAssets._Scripts.GameScene
{
    public class PooledParticleSystem : NetworkBehaviour
    {
        private void OnParticleSystemStopped()
        {
            NetworkObject.Despawn();
        }
    }
}