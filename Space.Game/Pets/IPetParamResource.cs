namespace Nebula.Pets {
    public interface IPetParamResource {
        float BaseValue();
        float ColorMult(PetInfo pet);
        float ColorValue();
        float LevelValue();
        float BaseMin();
        float BaseMax();
        float ColorMin();
        float ColorMax();
        float LevelMin();
        float LevelMax();
        //int Level(PetInfo pet);
    }
}
