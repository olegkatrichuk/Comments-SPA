using Comments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comments.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(c => c.HomePage)
            .HasMaxLength(2048);

        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(8000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Self-referencing relationship
        builder.HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // One-to-one with Attachment
        builder.HasOne(c => c.Attachment)
            .WithOne(a => a.Comment)
            .HasForeignKey<Attachment>(a => a.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore DomainEvents (not persisted)
        builder.Ignore(c => c.DomainEvents);

        // Indexes
        builder.HasIndex(c => c.CreatedAt)
            .IsDescending()
            .HasDatabaseName("ix_comments_created_at_desc");

        builder.HasIndex(c => c.UserName)
            .HasDatabaseName("ix_comments_user_name");

        builder.HasIndex(c => c.Email)
            .HasDatabaseName("ix_comments_email");

        builder.HasIndex(c => c.ParentCommentId)
            .HasDatabaseName("ix_comments_parent_comment_id");

        // Filtered index for top-level comments
        builder.HasIndex(c => c.CreatedAt)
            .IsDescending()
            .HasFilter("\"ParentCommentId\" IS NULL")
            .HasDatabaseName("ix_comments_top_level_created_at_desc");
    }
}
