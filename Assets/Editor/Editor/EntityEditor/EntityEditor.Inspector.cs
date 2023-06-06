using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace EntityEditor {
    public class Inspector {
        //Get this into standalone class
        Color outline = new Color(0.2f, 0.2f, 0.2f, 1f);
        Color darkBackground = new Color(0.102f, 0.102f, 0.102f);
        Color background = new Color(0.15f, 0.15f, 0.15f, 1f);
        public Inspector(EntityEditor editor, float width, float offset) {
            this.editor = editor;
            this.width = width;
            this.offset = offset;
            
        }

        public float width { get; set; }
        private float offset { get; set; }
        private EntityEditor editor;
        private PropertyBuilder propertyBuilder = new PropertyBuilder();
        private BindingFlags BindingFlags =
                             BindingFlags.Instance
                           | BindingFlags.Static
                           | BindingFlags.NonPublic
                           | BindingFlags.Public;

        private Func<string, int, int> intSerializationDelegate = (name, value) => { return EditorGUILayout.IntField(name, value, GUILayout.Height(EditorGUIUtility.singleLineHeight)); };
        private Func<string, float, float> floatSerializationDelegate = (name, value) => { return EditorGUILayout.FloatField(name, value, GUILayout.Height(EditorGUIUtility.singleLineHeight)); };
        private Func<string, double, double> doubleSerializationDelegate = (name, value) => { return EditorGUILayout.DoubleField(name, value, GUILayout.Height(EditorGUIUtility.singleLineHeight)); };
        private Func<string, string, string> stringSerializationDelegate = (name, value) => { return EditorGUILayout.TextField(name, value, GUILayout.Height(EditorGUIUtility.singleLineHeight)); };
        private Func<string, bool, bool> boolSerializationDelegate = (name, value) => { return EditorGUILayout.Toggle(name, value, GUILayout.Height(EditorGUIUtility.singleLineHeight)); };
        private Func<string, Vector2Int, Vector2Int> vector2IntSerializationDelegate = (name, value) => { return EditorGUILayout.Vector2IntField(name, value, GUILayout.Height(EditorGUIUtility.singleLineHeight)); };
        private Func<string, Vector2, Vector2> vector2SerializationDelegate = (name, value) => { return EditorGUILayout.Vector2Field(name, value, GUILayout.Height(EditorGUIUtility.singleLineHeight)); };
        private int currentKey = 0;
        public void Draw() {
            EditorGUIUtility.wideMode = true;
            InspectorHead();
            InspectorMenu();
        }

        private void InspectorHead() {
            float xBase = Screen.width - width;
            float baseoffset = 20;
            Rect textArea = new Rect(xBase + offset + baseoffset, offset, width - 2 * offset - baseoffset, 20);
            if (editor.entity == null) return;
            GUILayout.BeginArea(textArea);
            editor.entity.name = EditorGUILayout.TextField("Name: ", editor.entity.name, options: null);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(xBase + offset + baseoffset, 1.5f * offset + 20, width - 2 * offset - baseoffset, 20));
            editor.entity.icon = EditorGUILayout.ObjectField("Icon: ", editor.entity.icon, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
            GUILayout.EndArea();
        }

        private void InspectorMenu() {
            if (editor.entity == null) return;
            float space = 40;
            currentKey = 0;
            
            foreach (KeyValuePair<Type, Component> kvp in editor.entity.components) {
                space = DrawComponent(kvp.Value, width, space);
            }
        }

        private float DrawComponent(Component component, float width, float spaceOffset) {
            float space = spaceOffset + 20;
            Rect componentBoxRect = new Rect(Screen.width - width + 2 * offset, space, width - 2 * offset, 20);
            Rect componentLabelRect = new Rect(Screen.width - width + 2.5f * offset, space, width - 3 * offset, 20);
            Handles.DrawSolidRectangleWithOutline(componentBoxRect, darkBackground, outline);
            EditorGUI.LabelField(componentLabelRect, component.GetType().Name);
            space = DrawSerializedComponent(component, space);
            return space;
        }

        private float DrawSerializedComponent(Component component, float spaceOffset) {
            float space = spaceOffset;
            Type type = component.GetType();
            System.Reflection.FieldInfo[] infos = type.GetFields(BindingFlags);
            System.Reflection.PropertyInfo[] properties = type.GetProperties(BindingFlags);
            int i = 0;
            foreach (System.Reflection.FieldInfo fieldInfo in infos) {
                Type fieldType = properties[i].PropertyType;
                object value = fieldInfo.GetValue(component);
                bool isInherited = fieldType.IsSubclassOf(typeof(UnityEngine.Object)) || fieldType == typeof(UnityEngine.Object);
                UnityEngine.Object obj = value as UnityEngine.Object;
                if (isInherited) {
                    space += 20;
                    GUILayout.BeginArea(new Rect(Screen.width - width + 3 * offset, space, width - 4 * offset, 20));
                    obj = EditorGUILayout.ObjectField(properties[i].Name, obj, fieldType, false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    fieldInfo.SetValue(component, obj);
                    GUILayout.EndArea();
                }
                
                space = SerializeInnerFields(fieldType,space, (handler, val) => { fieldInfo.SetValue(handler, val); }, component, value, properties[i].Name);

                bool isList = fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>);
                if (isList) {
                    IList enumerable = value as IList;
                    int size = enumerable.Count;
                    Type genericType = enumerable.GetType().GetGenericArguments()[0];
                    object newList = Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType));
                    int index = 0;
                    foreach (object thisObj in enumerable) {
                        Debug.Log(thisObj + " : " + thisObj.GetType());
                        space = SerializeInnerFields(genericType, space, (handler, val) => { (newList as IList).Add(val); }, null, thisObj, "Element " + index);
                        index++;
                    }
                    fieldInfo.SetValue(component, newList);
                }
                i++;
            }
            return space;
        }

        private float SerializeInnerFields(Type fieldType, float space, Action<object,object> call, object valueHolder, object value, string name) {

            if (fieldType == typeof(int)) space = SerializeField<int>(call, valueHolder, space, value, name, intSerializationDelegate);
            if (fieldType == typeof(float)) space = SerializeField<float>(call, valueHolder, space, value, name, floatSerializationDelegate);
            if (fieldType == typeof(double)) space = SerializeField<double>(call, valueHolder, space, value, name, doubleSerializationDelegate);
            if (fieldType == typeof(string)) space = SerializeField<string>(call, valueHolder, space, value, name, stringSerializationDelegate);
            if (fieldType == typeof(bool)) space = SerializeField<bool>(call, valueHolder, space, value, name, boolSerializationDelegate);
            if (fieldType == typeof(Vector2Int)) space = SerializeField<Vector2Int>(call, valueHolder, space, value, name, vector2IntSerializationDelegate);
            if (fieldType == typeof(Vector2)) space = SerializeField<Vector2>(call, valueHolder, space, value, name, vector2SerializationDelegate);
            if (fieldType == typeof(Address)) { 
                space += 25;
                Rect rect = new Rect(Screen.width - width + 3 * offset, space, width - 4 * offset, 20);
                propertyBuilder.DrawProperty(editor.rootVisualElement, rect, value, currentKey, name);
                call.Invoke(valueHolder, value);
                currentKey++;
            }
            return space;
        }

        private float SerializeField<T>(Action<object, object> setCall, object valueHolder, float space, object value, string name, Func<string,T,T> call) {
            space += 20;
            GUILayout.BeginArea(new Rect(Screen.width - width + 3 * offset, space, width - 4 * offset, 40));
            value = call.Invoke(name, (T)value);
            setCall.Invoke(valueHolder, value);
            GUILayout.EndArea();
            return space;
        }
    }
}
// Put this somewhere else, or, if not needed remove
public static class ListExtra {
    public static void Resize<T>(this List<T> list, int sz, T c) {
        int cur = list.Count;
        if (sz < cur)
            list.RemoveRange(sz, cur - sz);
        else if (sz > cur) {
            if (sz > list.Capacity) //this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                list.Capacity = sz;
            list.AddRange(Enumerable.Repeat(c, sz - cur));
        }
    }
    public static void Resize<T>(this List<T> list, int sz) where T : new() {
        Resize(list, sz, new T());
    }
    public static List<T> SetPropertyValue<T>(object obj, object value) {
        List<T> listValue = value.ToString()
                .Split(',')
                .Select(x => (T)Convert.ChangeType(x, typeof(T), null))
                .ToList();
        return listValue;
    }
}
//Put this somewhere else
public class PropertyBuilder {
    private Dictionary<int, DrawnData> objectsDrawn = new Dictionary<int, DrawnData>();
    public void DrawProperty(VisualElement root, Rect rect, object prop, int id, string name) {
        if (objectsDrawn.ContainsKey(id)) {
            if (objectsDrawn[id].rect.x != rect.x) {
                objectsDrawn[id].element.transform.position = new Vector2(rect.x, rect.y);
                objectsDrawn[id].element.style.width = new StyleLength(rect.width);
                objectsDrawn[id].element.style.height = new StyleLength(rect.height);
            }
            return;
        }
        GenericProperty instance = ScriptableObject.CreateInstance<GenericProperty>();
        instance.Name = name;
        instance.property = prop;
        SerializedObject serializedObject = new SerializedObject(instance);
        PropertyField property = new PropertyField();
        property.bindingPath = nameof(instance.property);
        property.Bind(serializedObject);
        property.transform.position = new Vector2(rect.x,rect.y);
        property.style.width = new StyleLength(rect.width);
        property.style.height = new StyleLength(rect.height);
        root.Add(property);
        objectsDrawn.Add(id, new DrawnData(rect, property));

    }
    private class DrawnData {
        public Rect rect;
        public VisualElement element;
        public DrawnData(Rect rect, VisualElement element) {
            this.rect = rect;
            this.element = element;
        }
    }
}


