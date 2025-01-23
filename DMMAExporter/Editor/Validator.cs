using System.Collections.Generic;

using UnityEngine;

namespace DMMAExporter
{
    public class Validator
    {
        private readonly comparers.ErrorComparer errorComparer;
        public Validator()
        {
            errorComparer = new comparers.ErrorComparer();
        }
        public void ValidateSelectedModel(List<Error> errors, GameObject model)
        {
            if (!model.TryGetComponent<Animator>(out var animator))
                errors.Add(new Error("Model doesn't have an animator, can't generate bundle.", ErrorType.Error));
            else if (!animator.avatar.isHuman)
                errors.Add(new Error("Model is not humanoid, can't generate bundle. Model is not humanoid, can't generate bundle. Model is not humanoid, can't generate bundle.", ErrorType.Error));

            #if !UNITY_2022_3
                errors.Add(new Error("Unity version not supported, please install Unity 2022.3.", ErrorType.Error));
            #endif

            errors.Sort(errorComparer);
        }
    }
}