using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Common
{
    public class AdoTemplate {
        private readonly IConnectionFactory connectionFactory;

        public AdoTemplate(IConnectionFactory connectionFactory) {
            this.connectionFactory = connectionFactory;
        }

        public IEnumerable<T> Query<T>(string sql, RowMapper<T> rowMapper) {
            return Query(sql, new QueryParameter[] { }, rowMapper);
        }

        public IEnumerable<T> Query<T>(string sql, QueryParameter[] queryParameters, RowMapper<T> rowMapper) {
            using (DbConnection connection = connectionFactory.CreateConnection()) {
                using (DbCommand command = connection.CreateCommand()) {
                    command.Connection = connection;
                    command.CommandText = sql;

                    AddParameters(queryParameters, command);

                    var items = new List<T>();
                    using (IDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            items.Add(rowMapper(reader));
                        }
                    }

                    return items;
                }
            }
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, RowMapper<T> rowMapper) {
            return QueryAsync(sql, new QueryParameter[] { }, rowMapper);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, QueryParameter[] queryParameters, RowMapper<T> rowMapper) {
            using (DbConnection connection = await connectionFactory.CreateConnectionAsync()) {
                using (DbCommand command = connection.CreateCommand()) {
                    command.Connection = connection;
                    command.CommandText = sql;

                    AddParameters(queryParameters, command);

                    var items = new List<T>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            items.Add(rowMapper(reader));
                        }
                    }

                    return items;
                }
            }
        }

        public int Execute(string sql, QueryParameter[] parameters) {
            using (DbConnection connection = connectionFactory.CreateConnection()) {
                using (DbCommand command = connection.CreateCommand()) {
                    command.Connection = connection;
                    command.CommandText = sql;

                    AddParameters(parameters, command);

                    return command.ExecuteNonQuery();
                }
            }
        }

        public async Task<int> ExecuteAsync(string sql, QueryParameter[] parameters) {
            using (DbConnection connection = await connectionFactory.CreateConnectionAsync()) {
                using (DbCommand command = connection.CreateCommand()) {
                    command.Connection = connection;
                    command.CommandText = sql;

                    AddParameters(parameters, command);

                    return command.ExecuteNonQuery();
                }
            }
        }

        private void AddParameters(QueryParameter[] parameters, DbCommand command) {
            foreach (var p in parameters) {
                DbParameter dbParameter = command.CreateParameter();
                dbParameter.ParameterName = p.Name;
                dbParameter.Value = p.Value;

                command.Parameters.Add(dbParameter);
            }
        }
    }
}
