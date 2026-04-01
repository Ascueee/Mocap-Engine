using Galaxy3D.ECS.Systems;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using OpenTK.Mathematics;

namespace Galaxy3D.EngineSpecific.ECS.Systems.Rendering;

public class AnimationSystem : ISystem
{
    private Entity[] _animationEntities;
    private readonly IdGenerator _idGenerator;
    private int _currentSystemAmount;
    private int _maxEntities = 100;
    private ECSWorld _world;

    public AnimationSystem(int MAX_ENTITIES)
    {
        _animationEntities = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
    }
    
    public void UpdateSystem(float deltaTime,ECSWorld world)
    {
        _world = world;
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            Entity e = _animationEntities[i];
            Animator entityAnimator = e.GetComponent<Animator>();

            if (entityAnimator.layers.Count == 0) continue;

            Dictionary<string, Vector3> accumulatedRotations = new Dictionary<string, Vector3>();

            // Higher layers run first so they claim their entities first
            foreach (int layer in entityAnimator.layers.Keys.OrderByDescending(l => l))
            {
                string animName = entityAnimator.layers[layer];
                if (!entityAnimator.animations.ContainsKey(animName)) continue;

                Animation currentAnimation = entityAnimator.animations[animName];

                // Advance this layer's time
                float layerTime = entityAnimator.layerTimes[layer];
                layerTime += deltaTime * entityAnimator.layerSpeeds[layer];
                if (currentAnimation.isLooping)
                    layerTime %= currentAnimation.AnimationDuration;
                entityAnimator.layerTimes[layer] = layerTime;

                foreach (var pair in currentAnimation.AnimatedEntities)
                {
                    string entityName = pair.Key;

                    // Higher layer already claimed this entity — skip
                    if (accumulatedRotations.ContainsKey(entityName)) continue;

                    List<Keyframe> keyframes = pair.Value;

                    Keyframe from = keyframes[0];
                    Keyframe to   = keyframes[keyframes.Count - 1];

                    for (int k = 0; k < keyframes.Count - 1; k++)
                    {
                        if (layerTime >= keyframes[k].time && layerTime <= keyframes[k + 1].time)
                        {
                            from = keyframes[k];
                            to   = keyframes[k + 1];
                            break;
                        }
                    }

                    float t = (to.time - from.time) == 0 ? 0 :
                        (layerTime - from.time) / (to.time - from.time);
                    t = Math.Clamp(t, 0f, 1f);

                    accumulatedRotations[entityName] = Vector3.Lerp(from.rotation, to.rotation, t);
                }
            }

            // Apply all rotations
            foreach (var pair in accumulatedRotations)
            {
                if (!_world.EntityExists(pair.Key)) continue;
                Entity targetEntity = _world.GetEntity(pair.Key);
                Transform transform = targetEntity.GetComponent<Transform>();
                transform.rotation = pair.Value;
                transform.isDirty = true;
                targetEntity.SetComponent(transform);
            }

            e.SetComponent(entityAnimator);
        }
    }

    public void AddEntityToSystem(Entity e)
    {
        if (_currentSystemAmount == _animationEntities.Length)
        {
            _maxEntities += 100;
            _idGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new Entity[_maxEntities];
            _animationEntities.CopyTo(updatedArray, 0);
            _animationEntities = updatedArray;
        }

        var id = _idGenerator.CreateEntityID();
        _animationEntities[id] = e;
        _currentSystemAmount++;
    }
}