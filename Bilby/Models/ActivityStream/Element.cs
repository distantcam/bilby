using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bilby.ActivityStream;

[JsonConverter(typeof(ElementJsonConverter<Element>))]
public partial class Element : IEquatable<Element>, IEnumerable
{
    private int _index;
    private Dictionary<string, Element>? _value1;
    private List<Element>? _value2;
    private string? _value3;
    private bool? _value4;
    private int? _value5;

    public Element()
    {
    }

    protected Element(
        int index,
        Dictionary<string, Element>? value1 = default,
        List<Element>? value2 = default,
        string? value3 = default,
        bool? value4 = default,
        int? value5 = default)
    {
        Convert(index, value1, value2, value3, value4, value5);
    }

    private void Convert(Dictionary<string, Element> value) => Convert(1, value1: value);
    private void Convert(List<Element> value) => Convert(2, value2: value);
    private void Convert(string value) => Convert(3, value3: value);
    private void Convert(bool value) => Convert(4, value4: value);
    private void Convert(int value) => Convert(5, value5: value);

    private void Convert(
        int index,
        Dictionary<string, Element>? value1 = default,
        List<Element>? value2 = default,
        string? value3 = default,
        bool? value4 = default,
        int? value5 = default)
    {
        _index = index;
        _value1 = value1;
        _value2 = value2;
        _value3 = value3;
        _value4 = value4;
        _value5 = value5;
    }

    public bool IsObject => _index == 1;
    public bool IsArray => _index == 2;
    public bool IsString => _index == 3;
    public bool IsBool => _index == 4;
    public bool IsInt => _index == 5;

    public Dictionary<string, Element> AsObject() =>
        _index == 1 ? _value1! : throw new InvalidCastException("Element is not an object");
    public List<Element> AsArray() =>
        _index == 2 ? _value2! : throw new InvalidCastException("Element is not an array");
    public string AsString() =>
        _index == 3 ? _value3! : throw new InvalidCastException("Element is not a string");
    public bool AsBool() =>
        _index == 4 ? _value4!.Value : throw new InvalidCastException("Element is not a bool");
    public int AsInt() =>
        _index == 5 ? _value5!.Value : throw new InvalidCastException("Element is not an int");

    public void Switch(
        Action<Dictionary<string, Element>> objectAction,
        Action<List<Element>> arrayAction,
        Action<string> stringAction,
        Action<bool> boolAction,
        Action<int> intAction)
    {
        if (_index == 1 && objectAction != null)
        {
            objectAction(_value1!);
            return;
        }
        if (_index == 2 && arrayAction != null)
        {
            arrayAction(_value2!);
            return;
        }
        if (_index == 3 && stringAction != null)
        {
            stringAction(_value3!);
            return;
        }
        if (_index == 4 && boolAction != null)
        {
            boolAction(_value4!.Value);
            return;
        }
        if (_index == 5 && intAction != null)
        {
            intAction(_value5!.Value);
            return;
        }
        throw new InvalidOperationException();
    }

    public TResult Match<TResult>(
        Func<Dictionary<string, Element>, TResult> objectFunc,
        Func<List<Element>, TResult> arrayFunc,
        Func<string, TResult> stringFunc,
        Func<bool, TResult> boolFunc,
        Func<int, TResult> intFunc)
    {
        if (_index == 1 && objectFunc != null)
        {
            return objectFunc(_value1!);
        }
        if (_index == 2 && arrayFunc != null)
        {
            return arrayFunc(_value2!);
        }
        if (_index == 3 && stringFunc != null)
        {
            return stringFunc(_value3!);
        }
        if (_index == 4 && boolFunc != null)
        {
            return boolFunc(_value4!.Value);
        }
        if (_index == 5 && intFunc != null)
        {
            return intFunc(_value5!.Value);
        }
        throw new InvalidOperationException();
    }

    public Element this[string key]
    {
        get => _index == 1 ? _value1![key] : throw new InvalidCastException("Element is not an object");
        set => _value1![key] = value;
    }

    public Element this[int index]
    {
        get => _index == 2 ? _value2![index] : throw new InvalidCastException("Element is not an array");
        set => _value2![index] = value;
    }

    public void Add(string key, Element value)
    {
        if (_index == 0)
        {
            // Initialize as an object
            _index = 1;
            _value1 = new() { { key, value } };
        }
        else if (_index == 1)
        {
            _value1!.Add(key, value);
        }
        else
        {
            throw new InvalidCastException("Element is not an object");
        }
    }

    public void Add(Element value)
    {
        if (_index == 0)
        {
            // Initialize as an array
            _index = 2;
            _value2 = [value];
        }
        else if (_index == 2)
        {
            _value2!.Add(value);
        }
        else
        {
            throw new InvalidCastException("Element is not an array");
        }
    }

    public IEnumerator GetEnumerator() => _index switch
    {
        1 => _value1!.GetEnumerator(),
        2 => _value2!.GetEnumerator(),
        _ => throw new InvalidOperationException("Unexpected index")
    };

    public bool Equals(Element? other) =>
        other is not null &&
        _index == other._index &&
        _index switch
        {
            1 => Equals(_value1, other._value1),
            2 => Equals(_value2, other._value2),
            3 => Equals(_value3, other._value3),
            4 => Equals(_value4, other._value4),
            5 => Equals(_value5, other._value5),
            _ => false
        };

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = _index switch
            {
                1 => _value1!.GetHashCode(),
                2 => _value2!.GetHashCode(),
                3 => _value3!.GetHashCode(),
                4 => _value4!.GetHashCode(),
                5 => _value5!.GetHashCode(),
                _ => 0
            };
            return (hash * 397) ^ _index;
        }
    }
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is not null && obj is Element e && Equals(e);
    public override string? ToString() => _index switch
    {
        1 => _value1!.ToString(),
        2 => _value2!.ToString(),
        3 => _value3!.ToString(),
        4 => _value4!.ToString(),
        5 => _value5!.ToString(),
        _ => throw new InvalidOperationException("Unexpected index")
    };

    public static implicit operator Element(Dictionary<string, Element> value) => new(1, value1: value);
    public static implicit operator Element(List<Element> value) => new(2, value2: value);
    public static implicit operator Element(string value) => new(3, value3: value);
    public static implicit operator Element(bool value) => new(4, value4: value);
    public static implicit operator Element(int value) => new(5, value5: value);

    public static explicit operator Dictionary<string, Element>(Element element) => element.AsObject();
    public static explicit operator List<Element>(Element element) => element.AsArray();
    public static explicit operator string(Element element) => element.AsString();
    public static explicit operator bool(Element element) => element.AsBool();
    public static explicit operator int(Element element) => element.AsInt();

    public static bool operator ==(Element? left, Element? right) =>
        left is null || left.Equals(right);
    public static bool operator !=(Element? left, Element? right) => !(left == right);
}
