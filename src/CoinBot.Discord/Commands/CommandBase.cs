using System;
using Discord;
using Discord.Commands;

namespace CoinBot.Discord.Commands;

public abstract class CommandBase : ModuleBase
{
    protected static void AddAuthor(EmbedBuilder builder)
    {
        builder.WithAuthor(new EmbedAuthorBuilder
                           {
                               Name = "FunFair CoinBot - right click above to block",
                               Url = "https://funfair.io",
                               IconUrl = "https://s2.coinmarketcap.com/static/img/coins/32x32/1757.png"
                           });
    }

    protected static void AddFooter(EmbedBuilder builder, DateTime? dateTime = null)
    {
        if (dateTime.HasValue)
        {
            builder.Timestamp = dateTime;
            builder.Footer = new() { Text = "Prices updated" };
        }
    }
}