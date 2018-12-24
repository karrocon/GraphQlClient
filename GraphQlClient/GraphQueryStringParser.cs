using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQlClient
{
    public class GraphQueryStringParser
    {
        public static IGraphQueryableObject Parse(string queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            var (parsedObject, index) = ParseObject(queryString, 0);

            if (index != queryString.Length)
            {
                throw new Exception($"Unexpected ending: The root object was completely parsed at {index} but the query string size is {queryString.Length}");
            }

            return parsedObject;
        }

        private static (IGraphQueryableObject operation, int index) ParseRoot(string queryString)
        {
            var index = 0;

            while (index < queryString.Length)
            {
                var character = queryString[index];

                if (character == '{')
                {
                    return ParseObject(queryString, index);
                }
                else if (char.IsLetter(character))
                {
                    var (word, newIndex) = ParseWord(queryString, index);
                    switch (word)
                    {
                        case "query":
                            return ParseObject(queryString, newIndex);
                        case "mutation":
                            return ParseMutation(queryString, newIndex);
                        default:
                            throw new Exception($"Unsupported operation: {word}");
                    }
                }
                else if (!char.IsSeparator(character))
                {
                    throw new Exception($"Unexpected character at {index}");
                }

                index++;
            }

            throw new Exception($"End-of-string: Cannot parse operation");
        }

        private static (GraphQueryableObject<object> @object, int index) ParseMutation(string operationString, int startingIndex)
        {
            throw new NotImplementedException();
        }

        private static (GraphQueryableField field, int index) ParseField(string queryString, int startingIndex)
        {
            var (fieldName, index) = ParseWord(queryString, startingIndex);
            GraphQueryableField field = new GraphQueryableScalar(fieldName);

            IEnumerable<GraphQueryableArgument> arguments = null;
            while (index < queryString.Length)
            {
                var character = queryString[index];
                if (character == '{')
                {
                    var (@object, newIndex) = ParseObject(queryString, index);

                    @object.Name = fieldName;
                    if (arguments != null)
                    {
                        foreach (var argument in arguments)
                        {
                            @object.AddArgument(argument);
                        }
                    }

                    return (@object, newIndex);
                }
                else if (character == '(')
                {
                    if (arguments != null)
                    {
                        throw new Exception($"Unexpected character: Found wrong character at {index}");
                    }

                    var (parsedArguments, newIndex) = ParseArguments(queryString, index);

                    arguments = parsedArguments;
                    index = newIndex;
                }
                else if (char.IsLetter(character) || character == ',' || character == '}')
                {
                    // Missing arguments in field
                    return character == ','
                        ? (field, index + 1)
                        : (field, index);
                }
                else if (!char.IsSeparator(character))
                {
                    throw new Exception($"Unexpected character: Found wrong character at {index}");
                }

                index++;
            }

            throw new Exception($"End-of-string: Cannot parse word at {startingIndex}");
        }

        private static (GraphQueryableObject<object> @object, int index) ParseObject(string queryString, int startingIndex)
        {
            var index = startingIndex;
            GraphQueryableObject<object> @object = null;
            while (index < queryString.Length)
            {
                var character = queryString[index];

                if (@object == null)
                {
                    if (character == '{')
                    {
                        @object = new GraphQueryableObject<object>(string.Empty);
                    }
                    else if (!char.IsSeparator(character))
                    {
                        throw new Exception($"Unexpected character: Found wrong character at {index}");
                    }
                }
                else
                {
                    if (character == '}')
                    {
                        return (@object, index + 1);
                    }
                    else if (character == ',')
                    {
                        var (field, newIndex) = ParseField(queryString, index + 1);

                        if (field is GraphQueryableScalar)
                        {
                            @object.AddScalar((GraphQueryableScalar)field);
                        }
                        else
                        {
                            @object.AddObject((GraphQueryableObject<object>)field);
                        }

                        index = newIndex - 1;
                    }
                    else if (char.IsLetterOrDigit(character))
                    {
                        var (field, newIndex) = ParseField(queryString, index);

                        if (field is GraphQueryableScalar)
                        {
                            @object.AddScalar((GraphQueryableScalar)field);
                        }
                        else
                        {
                            @object.AddObject((GraphQueryableObject<object>)field);
                        }

                        index = newIndex - 1;
                    }
                    else if (!char.IsSeparator(character))
                    {
                        throw new Exception($"Unexpected character: Found wrong character at {index}");
                    }
                }

                index++;
            }

            throw new Exception($"End-of-string: Cannot parse object at {startingIndex}");
        }

        private static (IEnumerable<GraphQueryableArgument> arguments, int index) ParseArguments(string queryString, int startingIndex)
        {
            var index = startingIndex;
            var arguments = new List<GraphQueryableArgument>();
            while (index < queryString.Length)
            {
                var character = queryString[index];
                if (arguments.Count == 0)
                {
                    if (character == '(')
                    {
                        var (argument, newIndex) = ParseArgument(queryString, index + 1);
                        arguments.Add(argument);
                        index = newIndex - 1;
                    }
                    else if (!char.IsSeparator(character))
                    {
                        throw new Exception($"Unexpected character: Found wrong character at {index}");
                    }
                }
                else
                {
                    if (character == ')')
                    {
                        return (arguments, index);
                    }
                    else if (character == ',')
                    {
                        var (argument, newIndex) = ParseArgument(queryString, index + 1);
                        arguments.Add(argument);
                        index = newIndex - 1;
                    }
                    else if (!char.IsSeparator(character))
                    {
                        throw new Exception($"Unexpected character: Found wrong character at {index}");
                    }
                }

                index++;
            }

            throw new Exception($"End-of-string: Cannot parse arguments at {startingIndex}");
        }

        private static (GraphQueryableArgument argument, int index) ParseArgument(string queryString, int startingIndex)
        {
            var (argumentName, index) = ParseWord(queryString, startingIndex);

            if (queryString[index] != ':')
            {
                throw new Exception($"Expected character not found: It was expected : but found {queryString[index]} instead at {index}");
            }

            index++;

            var (argumentValue, newIndex) = ParseArgumentValue(queryString, index);

            return (new GraphQueryableArgument(argumentName, argumentValue), newIndex);
        }

        private static (GraphQueryableArgumentValue argumentValue, int index) ParseArgumentValue(string queryString, int startingIndex)
        {
            if (queryString[startingIndex] == '$')
            {
                var (variableName, index) = ParseWord(queryString, startingIndex + 1);
                return (new GraphQueryableVariable(variableName), index);
            }
            else
            {
                var (literal, index) = ParseLiteral(queryString, startingIndex);
                return (literal, index);
            }
        }

        private static (GraphQueryableLiteral<object> literal, int index) ParseLiteral(string queryString, int startingIndex)
        {
            var index = startingIndex + 1;
            var character = queryString[index - 1];
            var stringBuilder = new StringBuilder(character.ToString());
            if (character == '"')
            {
                stringBuilder.Remove(0, 1);
                while (index < queryString.Length)
                {
                    character = queryString[index];

                    if (character == '"')
                    {
                        return (new GraphQueryableLiteral<object>(stringBuilder.ToString()), index + 1);
                    }

                    stringBuilder.Append(character);
                    index++;
                }

                throw new Exception($"End-of-string: Cannot parse string literal at {startingIndex}");
            }
            else if (char.IsNumber(character))
            {
                while (index < queryString.Length)
                {
                    character = queryString[index];

                    if (character == ',' || character == ')')
                    {
                        return (new GraphQueryableLiteral<object>(JsonConvert.DeserializeObject(stringBuilder.ToString())), index);
                    }

                    stringBuilder.Append(character);
                    index++;
                }

                throw new Exception($"End-of-string: Cannot parse string literal at {startingIndex}");
            }
            else if (character == '[')
            {
                while (index < queryString.Length)
                {
                    character = queryString[index];

                    if (character == ']')
                    {
                        return (new GraphQueryableLiteral<object>(JsonConvert.DeserializeObject(stringBuilder.ToString())), index);
                    }

                    stringBuilder.Append(character);
                    index++;
                }

                throw new Exception($"End-of-string: Cannot parse string literal at {startingIndex}");
            }
            else if (character == '{')
            {
                while (index < queryString.Length)
                {
                    character = queryString[index];

                    if (character == '}')
                    {
                        return (new GraphQueryableLiteral<object>(JsonConvert.DeserializeObject(stringBuilder.ToString())), index);
                    }

                    stringBuilder.Append(character);
                    index++;
                }

                throw new Exception($"End-of-string: Cannot parse string literal at {startingIndex}");
            }

            throw new Exception($"Unexpected character: Found wrong character at {startingIndex}");
        }

        private static (string word, int index) ParseWord(string queryString, int startingIndex)
        {
            var index = startingIndex;
            var stringBuilder = new StringBuilder();
            while (index < queryString.Length)
            {
                var character = queryString[index];
                if (char.IsLetterOrDigit(character))
                {
                    stringBuilder.Append(character);
                }
                else if (stringBuilder.Length > 0 && (char.IsSeparator(character) || character == ',' || character == '(' || character == '{' || character == ':' || character == '}' || character == ')'))
                {
                    return (stringBuilder.ToString(), index);
                }
                else if (stringBuilder.Length != 0 || !char.IsSeparator(character))
                {
                    throw new Exception($"Unexpected character: Found wrong character at {index}");
                }

                index++;
            }

            throw new Exception($"End-of-string: Cannot parse word at {startingIndex}");
        }
    }
}
