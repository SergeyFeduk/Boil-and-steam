using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class EntitySerializer {
    private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();
    private readonly Serializer serializer;
    private readonly Deserializer deserializer;

    public EntitySerializer() {
        LoadSurrogates();
        serializer = new Serializer(binaryFormatter);
        deserializer = new Deserializer(binaryFormatter);
    }

    public string Serialize(object data) {
        return serializer.Serialize(data);
    }

    public object Deserialize(string data) {
        return deserializer.Deserialize(data);
    }

    private void LoadSurrogates() {
        SurrogateSelector surrogateSelector = new SurrogateSelector();

        Vector2IntSerializationSurrogate vector2intSS = new Vector2IntSerializationSurrogate();
        surrogateSelector.AddSurrogate(typeof(Vector2Int), new StreamingContext(StreamingContextStates.All), vector2intSS);
        Vector2SerializationSurrogate vector2SS = new Vector2SerializationSurrogate();
        surrogateSelector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), vector2SS);

        binaryFormatter.SurrogateSelector = surrogateSelector;
    }
}
