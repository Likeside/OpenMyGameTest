using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts {
    
    [CreateAssetMenu(fileName = "AnimationConfigSO", menuName = "Configs/AnimationConfigSO", order = 1)]
    public class AnimationConfigSO: ScriptableObject {
        public List<AnimationConfig> animationConfigs;
        
        [Serializable]
        public class AnimationConfig {
            public int representation;
            public string idleTrigger;
            public string destroyTrigger;
        }
    }
}