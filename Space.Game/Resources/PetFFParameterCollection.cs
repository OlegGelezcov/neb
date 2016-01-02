
using Common;
using System.Collections.Concurrent;

namespace Nebula.Resources {
    public class PetFFParameterCollection {
        private readonly ConcurrentDictionary<PetColor, FloatFloatPetParameter> m_Parameters = new ConcurrentDictionary<PetColor, FloatFloatPetParameter>();

        public void AddParameter(FloatFloatPetParameter parameter ) {

            bool removedSuccess = true;
            if(m_Parameters.ContainsKey(parameter.color)) {
                FloatFloatPetParameter oldParameter;
                if (!m_Parameters.TryRemove(parameter.color, out oldParameter)) {
                    removedSuccess = false;
                }
            }

            if(removedSuccess) {
                m_Parameters.TryAdd(parameter.color, parameter);
            }
        }

        public FloatFloatPetParameter GetParameter(PetColor color) {
            if(m_Parameters.ContainsKey(color)) {
                FloatFloatPetParameter parameter;
                if(m_Parameters.TryGetValue(color, out parameter)) {
                    return parameter;
                }
            }
            return null;
        }
    }
}
