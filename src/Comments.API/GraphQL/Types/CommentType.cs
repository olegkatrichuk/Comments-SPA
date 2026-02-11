using Comments.Application.DTOs;

namespace Comments.API.GraphQL.Types;

public sealed class CommentType : ObjectType<CommentDto>
{
    protected override void Configure(IObjectTypeDescriptor<CommentDto> descriptor)
    {
        descriptor.Name("Comment");

        descriptor.Field(c => c.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(c => c.UserName).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Email).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.HomePage).Type<StringType>();
        descriptor.Field(c => c.Text).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(c => c.Attachment).Type<AttachmentType>();
        descriptor.Field(c => c.Replies).Type<NonNullType<ListType<NonNullType<CommentType>>>>();
    }
}
