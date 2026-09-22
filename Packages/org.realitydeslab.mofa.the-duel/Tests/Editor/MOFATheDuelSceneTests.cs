using HoloKit.Tests;
using NUnit.Framework;

namespace HoloKit.GameTests
{
    [TestFixture]
    public sealed class MOFATheDuelSceneTests : SceneContractTests
    {
        protected override string ScenePath => "Packages/org.realitydeslab.mofa.the-duel/Assets/Scenes/MOFATheDuel.unity";

        [Test]
        public void RequiredGameInteractionReferencesAreAssigned()
        {
            AssertRequiredReferences("SpellList", "LifeShieldList", "SpellPool");
        }
    }
}
