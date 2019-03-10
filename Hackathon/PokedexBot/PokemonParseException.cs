using System;
using System.Collections.Generic;
using System.Linq;

namespace PokeAPI
{
    /// <summary>
    /// An exception thrown when parsing a PokeApi JSON object.
    /// </summary>
    public class PokemonParseException : Exception
    {
        const string DEFAULT_MESSAGE = "An error occured when parsing Pokemon data.";

        /// <summary>
        /// Creates a new instance of the <see cref="PokemonParseException" /> class.
        /// </summary>
        public PokemonParseException() : this(DEFAULT_MESSAGE, null) { }
        /// <summary>
        /// Creates a new instance of the <see cref="PokemonParseException" /> class.
        /// </summary>
        /// <param name="message">The message of the <see cref="Exception" />.</param>
        public PokemonParseException(string message) : this(message, null) { }
        /// <summary>
        /// Creates a new instance of the <see cref="PokemonParseException" /> class.
        /// </summary>
        /// <param name="innerException">The <see cref="Exception" /> that was the cause of this <see cref="Exception" />.</param>
        public PokemonParseException(Exception innerException) : this(DEFAULT_MESSAGE, innerException) { }
        /// <summary>
        /// Creates a new instance of the <see cref="PokemonParseException" /> class.
        /// </summary>
        /// <param name="message">The message of the <see cref="Exception" />.</param>
        /// <param name="innerException">The <see cref="Exception" /> that was the cause of this <see cref="Exception" />.</param>
        public PokemonParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
