namespace MemeFight.Audio
{
    public class SoundEmitterPool : PoolInstance<SoundEmitter>
    {
        public override SoundEmitter Get()
        {
            Pool.Get(out SoundEmitter emitter);
            emitter.OnGetFromPool(this);
            return emitter;
        }

        public override void Return(SoundEmitter emitter)
        {
            Pool.Release(emitter);
        }
    }
}
