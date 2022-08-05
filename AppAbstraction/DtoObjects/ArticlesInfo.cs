using System.Collections.Generic;

namespace BBlog.Tests.AppAbstraction.DtoObjects;

public record ArticlesInfo(IEnumerable<Article> Articles, int ArticlesCount);