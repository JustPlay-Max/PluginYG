This is a little helper [PropertyDrawer](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html) that lets you hide properties unless another property has a true value. For example:

```cs
class MyBehavior : MonoBehavior
{
    public bool showSomeValue;

    [ConditionallyVisible(nameof(showSomeValue))]
    public float someValue;
}
```

The inspector will show the checkbox for `showSomeValue` and will only show the `someValue` field if you check the box.

This is handy if you're doing some kind of feature list or have fields that only apply when a value is toggled on.

![./example.gif](./example.gif)