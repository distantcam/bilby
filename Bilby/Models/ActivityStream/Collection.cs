using System.Text.Json.Serialization;
using Bilby.ActivityStream;

namespace Bilby.Models.ActivityStream;

[JsonConverter(typeof(ElementJsonConverter<Collection>))]
public class Collection : ASObject
{
    public Collection()
    {
        this["type"] = "Collection";
    }

    public Element TotalItems
    {
        get => this["totalItems"];
        set => this["totalItems"] = value;
    }

    public Element Current
    {
        get => this["current"];
        set => this["current"] = value;
    }

    public Element First
    {
        get => this["first"];
        set => this["first"] = value;
    }

    public Element Last
    {
        get => this["last"];
        set => this["last"] = value;
    }

    public Element Items
    {
        get => this["items"];
        set => this["items"] = value;
    }
}

[JsonConverter(typeof(ElementJsonConverter<OrderedCollection>))]
public class OrderedCollection : Collection
{
    public OrderedCollection()
    {
        this["type"] = "OrderedCollection";
    }
}

[JsonConverter(typeof(ElementJsonConverter<CollectionPage>))]
public class CollectionPage : Collection
{
    public CollectionPage()
    {
        this["type"] = "CollectionPage";
    }

    public Element PartOf
    {
        get => this["partOf"];
        set => this["partOf"] = value;
    }

    public Element Next
    {
        get => this["next"];
        set => this["next"] = value;
    }

    public Element Prev
    {
        get => this["prev"];
        set => this["prev"] = value;
    }
}

[JsonConverter(typeof(ElementJsonConverter<OrderedCollectionPage>))]
public class OrderedCollectionPage : Collection
{
    public OrderedCollectionPage()
    {
        this["type"] = "OrderedCollectionPage";
    }

    public Element PartOf
    {
        get => this["partOf"];
        set => this["partOf"] = value;
    }

    public Element Next
    {
        get => this["next"];
        set => this["next"] = value;
    }

    public Element Prev
    {
        get => this["prev"];
        set => this["prev"] = value;
    }

    public Element StartIndex
    {
        get => this["startIndex"];
        set => this["startIndex"] = value;
    }
}
