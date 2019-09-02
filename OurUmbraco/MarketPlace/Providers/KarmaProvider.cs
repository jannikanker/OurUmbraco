﻿using System;using System.Collections.Generic;
using System.Linq;
using OurUmbraco.MarketPlace.Interfaces;
using OurUmbraco.Our;
using Umbraco.Web;

namespace OurUmbraco.MarketPlace.Providers
{
    public class KarmaProvider : IKarmaProvider
    {
        public int GetProjectKarma(int projectId)
        {
            var db = UmbracoContext.Current.Application.DatabaseContext.Database;
            var result =  db.ExecuteScalar<int>("SELECT SUM(points) karma FROM powersProject WHERE id = @projectId", new { projectId = projectId });
            return result;
        }

        public IEnumerable<IKarma> GetProjectsKarmaList()
        {
            var karmaList = new List<IKarma>();

            using (var reader = Data.SqlHelper.ExecuteReader("SELECT id as ProjectId, SUM(points) Points FROM powersProject GROUP BY id ORDER BY SUM(points) DESC"))
            {
                while (reader.Read())
                {
                    var karma = new Karma
                    {
                        ProjectId = reader.GetInt("ProjectId"),
                        Points = reader.GetInt("Points")
                    };
                    karmaList.Add(karma);
                }
            }

            return karmaList;
        }

        public IEnumerable<IKarma> GetProjectsKarmaListByDate(DateTime afterDate)
        {
            throw new NotImplementedException();
        }

        public int AddKarma(IKarma karma)
        {
            throw new NotImplementedException();
        }

        public void ClearKarma(int projectId)
        {
            throw new NotImplementedException();
        }
    }
}
