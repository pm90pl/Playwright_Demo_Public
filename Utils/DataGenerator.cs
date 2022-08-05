using System;
using System.Linq;
using BBlog.Tests.AppAbstraction;
using BBlog.Tests.AppAbstraction.DtoObjects;

namespace BBlog.Tests.Utils;

public static class DataGenerator
{
    public static T GenerateData<T>() where T : class
    {
        var type = typeof(T);
        var data = type switch
        {
            { Name: nameof(Article) } => GenerateRandomArticle() as T,
            { Name: nameof(Comment) } => new Comment(){Body = Guid.NewGuid().ToString()} as T,

            _ => throw new ArgumentOutOfRangeException()
        };
        return data;
    }

    private static Article GenerateRandomArticle()
    {
        return new Article()
        {
            Title = Faker.Lorem.Sentence(4),
            Description = Faker.Lorem.Sentence(4),
            Body = Faker.Lorem.Paragraph(10),
            TagList = Faker.Lorem.Words(3).ToArray()
        };
    }
}