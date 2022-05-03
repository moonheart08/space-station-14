using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared.Conditionals;

[TypeSerializer]
public sealed class ConditionalSerializer:
    ITypeSerializer<Conditional, ValueDataNode>,
    ITypeSerializer<Conditional, SequenceDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return serializationManager.ValidateNode<bool>(node, context);
    }

    public Conditional Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies,
        bool skipHook, ISerializationContext? context = null, Conditional? value = default)
    {
        try
        {
            return new Conditional(serializationManager.Read<bool>(node, context, skipHook));
        }
        catch { /* ignored */ } // mfw exceptions

        throw new InvalidMappingException("The supplied value was not a boolean!");
    }

    public DataNode Write(ISerializationManager serializationManager, Conditional value, bool alwaysWrite = false,
        ISerializationContext? context = null)
    {
        throw new NotImplementedException();
    }

    public Conditional Copy(ISerializationManager serializationManager, Conditional source, Conditional target, bool skipHook,
        ISerializationContext? context = null)
    {
        throw new NotImplementedException();
    }

    public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return serializationManager.ValidateNode<List<ISimpleCondition>>(node, context);
    }

    public Conditional Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies,
        bool skipHook, ISerializationContext? context = null, Conditional? value = default)
    {
        try
        {
            return new Conditional(serializationManager.Read<List<ISimpleCondition>>(node, context, skipHook));
        }
        catch (Exception e) { Logger.Error($"Read error: {e}"); } // mfw exceptions

        throw new InvalidMappingException("The supplied value was not a condition list!");
    }
}
