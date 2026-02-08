using Comments.Application.DTOs;

namespace Comments.API.GraphQL.Types;

public sealed class AttachmentType : ObjectType<AttachmentDto>
{
    protected override void Configure(IObjectTypeDescriptor<AttachmentDto> descriptor)
    {
        descriptor.Name("Attachment");

        descriptor.Field(a => a.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(a => a.FileName).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.ContentType).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.Url).Type<NonNullType<StringType>>();
    }
}
