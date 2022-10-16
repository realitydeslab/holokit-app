using System.Collections.Generic;
using UnityEngine;

namespace Holoi.AssetFoundation
{
    public abstract class ArtifactCollection : ScriptableObject
    {
        public string BundleId;

        public abstract List<Artifact> Artifacts { get; }

        public abstract Artifact CoverArtifact { get; }

        public string DisplayName;

        public string Description;

        public string Author;

        public SmartContract Contract;

        public string ImageUrl;

        public abstract List<Tag> Tags { get; }
    }
}