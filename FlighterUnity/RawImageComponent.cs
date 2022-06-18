using UnityEngine;
using UnityEngine.UI;
using Component = Flighter.Component;

namespace FlighterUnity
{
    public class RawImageComponent : Component, IUnityFlighterComponent
    {
        public Texture Texture
        {
            get => image.texture;
            set => image.texture = value;
        }

        RawImage image;

        public void Clear()
        {
            UnityEngine.Object.Destroy(image);
            image = null;
        }

        public void InflateGameObject(GameObject gameObject)
        {
            image = gameObject.AddComponent<RawImage>();
        }
    }
}
