
using Common;
using System.Collections.Concurrent;

namespace Nebula.Resources {
    public class PetIFParameterCollection {
        private readonly ConcurrentDictionary<PetColor, IntFloatPetParameter> m_Parameters = new ConcurrentDictionary<PetColor, IntFloatPetParameter>();

        public void AddParameter(IntFloatPetParameter parameter) {

            bool removedSuccess = true;
            if (m_Parameters.ContainsKey(parameter.color)) {
                IntFloatPetParameter oldParameter;
                if (!m_Parameters.TryRemove(parameter.color, out oldParameter)) {
                    removedSuccess = false;
                }
            }

            if (removedSuccess) {
                m_Parameters.TryAdd(parameter.color, parameter);
            }
        }

        public IntFloatPetParameter GetParameter(PetColor color) {
            if (m_Parameters.ContainsKey(color)) {
                IntFloatPetParameter parameter;
                if (m_Parameters.TryGetValue(color, out parameter)) {
                    return parameter;
                }
            }
            return null;
        }
    }
}
