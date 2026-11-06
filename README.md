# Text to Form Fields

A small C# console app (.NET 6) that turns a plain-text description of a form into JSON field definitions.

## Run

Requires the .NET 6 runtime, or a newer SDK with it installed.

```
dotnet run --project TextToFields
```

Type one field per line and finish with a blank line, or pipe in a file:

```
dotnet run --project TextToFields < fields.txt
```

## Example

```
Full name
Phone number
Date of birth
Password
Age: number
Country dropdown [Pakistan, UAE]
Gender radio [Male] [Female]
```

The first and last fields come out as:

```json
[
  {
    "label": "Full name",
    "name": "full_name",
    "type": "text"
  },
  {
    "label": "Gender",
    "name": "gender",
    "type": "radio",
    "options": [
      "Male",
      "Female"
    ]
  }
]
```

## How it reads a line

- **Type:** the first word it recognises from `datetime`, `password`, `email`, `phone`, `date`, `dropdown`, `select`,
  `radio`, `checkbox`, `number` and `text`, in that order. With none of them, the field is `text`. `phone` becomes
  `tel`, and `dropdown` becomes `select`.
- **Explicit type:** `Age: number` uses the words after the colon for the type and those before it for the label.
- **Label:** the line without its options and trailing control words such as `dropdown` or `field`. Words like `date`
  stay, so `Birth date` keeps its full label.
- **Options:** anything in `[square brackets]`, separated by commas or in separate brackets. Only `select`, `radio` and
  `checkbox` fields use them.

It matches words with regular expressions, so it won't understand free-form sentences.
